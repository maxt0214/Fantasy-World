using System;
using System.Collections.Generic;
using Common.Data;
using Entities;
using UnityEngine;
using SkillBridge.Message;
using Common.Battle;
using Managers;
using Models;

namespace Battle
{
    public class Skill
    {
        public NSkillInfo Info;
        public Creature Owner;
        public SkillDefine Def;

        private float cd;
        public float CD
        {
            get { return cd; }
        }
        private float prepTime; //Time for casting
        private float castTime;
        public Creature Target;
        public NVector3 TargetPosition;

        public int HitCount;
        private SkillStatus Status;

        public bool isCasting = false;

        private List<Bullet> bullets = new List<Bullet>();
        private Dictionary<int, List<NDamageInfo>> HitMap = new Dictionary<int, List<NDamageInfo>>();

        public Skill(NSkillInfo info, Creature owner)
        {
            Info = info;
            Owner = owner;
            Def = DataManager.Instance.Skills[Owner.Define.TID][Info.Id];
            cd = 0;
        }

        public SkillResult Castable(Creature target, NVector3 pos)
        {
            if(Def.TargetType == TargetType.Single)
            {
                if (target == null || target == Owner)
                    return SkillResult.InvalidTarget;

                int distance = (int)Owner.DistanceTo(target);
                if (distance > Def.Range)
                    return SkillResult.OutOfRange;
            }

            if(Def.TargetType == TargetType.Area)
            {
                if (pos == null)
                    return SkillResult.InvalidTarget;

                int distance = (int)Owner.DistanceTo(pos.ToLogicInt());
                if (distance > Def.Range)
                    return SkillResult.OutOfRange;
            }

            if (cd > 0)
                return SkillResult.CoolDown;

            if (Owner.Attributes.MP < Def.MPCost)
                return SkillResult.InvalidMp;

            return SkillResult.Valid;
        }

        public void CastEffect(Creature target, NVector3 pos)
        {
            isCasting = true;
            prepTime = 0;
            castTime = 0;
            cd = Def.CD;
            Target = target;
            TargetPosition = pos;
            HitCount = 0;
            bullets.Clear();
            HitMap.Clear();

            if(Def.TargetType == TargetType.Area)
            {
                Owner.FaceTo(TargetPosition.ToLogicInt());
            } else if(Def.TargetType == TargetType.Single)
            {
                Owner.FaceTo(Target.position);
            }

            Owner.PlayAnim(Def.SkillAnim);

            if(Def.CastTime > 0)
            {
                Status = SkillStatus.Preparing;
            } else
            {
                CastSkillStarted();
            }
        }

        private void CastSkillStarted()
        {
            Status = SkillStatus.Casting;
            if (!string.IsNullOrEmpty(Def.AOEEffect))
            {
                if (Def.TargetType == TargetType.Area)
                    Owner.PlayEffect(EffectType.Position, Def.AOEEffect, TargetPosition);
                else if (Def.TargetType == TargetType.Single)
                    Owner.PlayEffect(EffectType.Position, Def.AOEEffect, Target);
                else if(Def.TargetType == TargetType.Self)
                    Owner.PlayEffect(EffectType.Position, Def.AOEEffect, Owner);
            }
        }

        public void OnUpdate(float delta)
        {
            UpdateCD(delta);

            if (Status == SkillStatus.Preparing)
            {
                UpdateCastPrep();
            }
            else if (Status == SkillStatus.Casting)
            {
                UpdateCastSkill();
            }
        }

        private void UpdateCastPrep()
        {
            if (prepTime < Def.CastTime)
            {
                prepTime += Time.deltaTime;
            }
            else
            {
                CastSkillStarted();
                Debug.LogFormat("Skill[{0}].UpdateCastPrep Over!", Def.Name);
            }
        }

        private void UpdateCastSkill()
        {
            castTime += Time.deltaTime;

            if (Def.Duration > 0)
            {
                if (castTime > Def.EffectInterval * (HitCount + 1))
                {
                    Hit();
                }

                if (castTime > Def.Duration)
                {
                    Status = SkillStatus.None;
                    isCasting = false;
                    Debug.LogFormat("Skill[{0}].UpdateCastSkill Over!", Def.Name);
                }
            }
            else if (Def.HitTimes != null && Def.HitTimes.Count > 0)
            {
                if (HitCount < Def.HitTimes.Count)
                {
                    if (castTime > Def.HitTimes[HitCount])
                    {
                        Hit();
                    }
                }
                else
                {
                    if (!Def.IfBullet)
                    {
                        Status = SkillStatus.None;
                        isCasting = false;
                        Debug.LogFormat("Skill[{0}].UpdateCastSkill Over!", Def.Name);
                    }
                }
            }

            if (Def.IfBullet)
            {
                bool finished = true;
                foreach (var bullet in bullets)
                {
                    bullet.Update();
                    if (!bullet.destroyed) finished = false;
                }

                if (finished && HitCount >= Def.HitTimes.Count)
                {
                    Status = SkillStatus.None;
                    isCasting = false;
                    Debug.LogFormat("Skill[{0}].UpdateCastSkill Over!", Def.Name);
                }
            }
        }

        private void UpdateCD(float delta)
        {
            if (cd > 0)
                cd = Mathf.Clamp(cd - delta, 0, Def.CD);
        }

        private void Hit()
        {
            if (Def.IfBullet)
            {
                CastBullet();
            }
            else
                DealHitDamage(HitCount);
            HitCount++;
        }

        public void DealHitDamage(int hit)
        {
            List<NDamageInfo> dmgs;
            if (HitMap.TryGetValue(hit, out dmgs))
            {
                DealHitDamage(dmgs);
            }
        }

        private void DealHitDamage(List<NDamageInfo> damages)
        {
            foreach (var dmg in damages)
            {
                var target = EntityManager.Instance.GetEntity(dmg.entityId) as Creature;
                if (target == null) continue;
                target.DealDamage(dmg,true);
                if(Def.HitEffect != null)
                {
                    target.PlayEffect(EffectType.Hit, Def.HitEffect,target);
                }
            }
        }

        private void CastBullet()
        {
            Bullet bullet = new Bullet(this);
            Debug.LogFormat("Skill[{0}].CastBullet[{1}]", Def.Name, Target.Name);
            bullets.Add(bullet);

            Owner.PlayEffect(EffectType.Bullet, Def.BulletRes, Target, bullet.duration);
        }

        public void InitHit(NHitInfo hitInfo)
        {
            //Only when bullet or non-bullet skill hits enemy, we init a hit.
            if(hitInfo.ifBullet || !Def.IfBullet)
            {
                InitHit(hitInfo.hitId, hitInfo.Damages);
            }
        }

        //Called by Creature to receive hit info from remote
        public void InitHit(int hitId, List<NDamageInfo> damages)
        {
            if (hitId > HitCount)
                HitMap[hitId] = damages;
            else
                DealHitDamage(damages);
        }

        public static string GetSkillErrorMessage(SkillResult skillResult)
        {
            switch (skillResult)
            {
                case SkillResult.CoolDown:
                    return "This Skill Is Cooling Down!";
                case SkillResult.InvalidMp:
                    return "Insufficient MP";
                case SkillResult.InvalidTarget:
                    return "Invalid Target";
                case SkillResult.OutOfRange:
                    return "Target Out Of Range";
                case SkillResult.InvalidSkill:
                    return "Invalid Skill: You Don't Have This Skill";
                case SkillResult.Casting:
                    return "This Skill Is Still In The Progress Of Casting";
            }

            return "";
        }
    }
}
