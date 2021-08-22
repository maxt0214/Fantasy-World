using Common;
using Common.Battle;
using Common.Data;
using Common.Utils;
using GameServer.Core;
using GameServer.Entities;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;

namespace GameServer.Battle
{
    class Skill
    {
        public NSkillInfo Info;
        public Creature Owner;
        public SkillDefine Def;

        public SkillStatus Status;

        private float cd;
        public float CD
        {
            get { return cd; }
        }

        public bool Instant
        {
            get
            {
                if (Def.CastTime > 0) return false;
                if (Def.IfBullet) return false;
                if (Def.Duration > 0) return false;
                if (Def.HitTimes != null && Def.HitTimes.Count > 0) return false;
                return true;
            }
        }

        private float prepTime = 0;
        private float castTime = 0;
        public int HitCount = 0;
        private BattleContext context;

        private List<Bullet> bullets = new List<Bullet>();

        public Skill(NSkillInfo info, Creature owner)
        {
            Info = info;
            Owner = owner;
            Def = DataManager.Instance.Skills[(int)Owner.Define.Class][Info.Id];
        }

        public SkillResult Castable(BattleContext context)
        {
            if (Status != SkillStatus.None)
                return SkillResult.Casting;

            if(Def.TargetType == TargetType.Single)
            {
                if (context.Target == null || context.Target == Owner)
                    return SkillResult.InvalidTarget;

                var distance = Owner.DistanceTo(context.Target);
                if(distance > Def.Range)
                    return SkillResult.OutOfRange;
            }

            if(Def.TargetType == Common.Battle.TargetType.Area)
            {
                if (context.CastingSkill.Position == null)
                    return SkillResult.InvalidTarget;
                if (Owner.DistanceTo(context.Position) > Def.Range)
                    return SkillResult.OutOfRange;
            }

            if (Owner.Attributes.MP < Def.MPCost)
                return SkillResult.InvalidMp;

            if (cd > 0)
                return SkillResult.CoolDown;

            return SkillResult.Valid;
        }

        public SkillResult Cast(BattleContext context)
        {
            var res = Castable(context);

            if(res == SkillResult.Valid)
            {
                prepTime = 0;
                castTime = 0;
                cd = Def.CD;
                HitCount = 0;
                this.context = context;
                bullets.Clear();

                AddBuff(TriggerType.SkillCast, context.Target);

                if (Instant)
                {
                    Hit();
                } else
                {
                    if (Def.CastTime > 0)
                    {
                        Status = SkillStatus.Preparing;
                    } else
                    {
                        Status = SkillStatus.Casting;
                    }
                }
            }
            Log.InfoFormat("Skill.Cast: Skill[{0}] Result:{1} Status:{2}", Def.Name, res, Status);
            return res;
        }

        private void Hit()
        {
            var hitInfo = InitHitInfo(false);
            HitCount++;

            if (Def.IfBullet)
            {
                CastBullet(hitInfo);
                return;
            }

            Hit(hitInfo);
        }

        public void Hit(NHitInfo hitInfo)
        {
            context.Battle.AddHitInfo(hitInfo);
            Log.InfoFormat("Skill[{0}].Hit[{1}] After:{2} Seconds  Bullet:{3}", Def.Name, HitCount, castTime, hitInfo.ifBullet);

            if (Def.AOERange > 0)
            {
                HitRange(hitInfo);
                return;
            }

            if(Def.TargetType == TargetType.Single)
            {
                HitTarget(context.Target, hitInfo);
            }
        }

        public NHitInfo InitHitInfo(bool bullet)
        {
            NHitInfo hitInfo = new NHitInfo();
            hitInfo.skillId = Def.ID;
            hitInfo.casterId = context.Caster.entityId;
            hitInfo.hitId = HitCount;
            hitInfo.ifBullet = bullet;

            return hitInfo;
        }

        private void CastBullet(NHitInfo hitInfo)
        {
            context.Battle.AddHitInfo(hitInfo);
            Log.InfoFormat("Skill[{0}].CastBullet[{1}]", Def.Name, context.Target.Name);
            Bullet bullet = new Bullet(this,context.Target, hitInfo);
            bullets.Add(bullet);
        }

        private void HitRange(NHitInfo hitInfo)
        {
            Vector3Int pos;
            if(Def.TargetType == TargetType.Single)
            {
                pos = context.Target.Position;
            } else if(Def.TargetType == Common.Battle.TargetType.Area)
            {
                pos = context.Position;
            } else
            {
                pos = Owner.Position;
            }

            List<Creature> hitted = context.Battle.FindMapEntitiesInRange(pos, Def.AOERange);
            foreach (var target in hitted)
            {
                HitTarget(target, hitInfo);
            }
        }

        private void HitTarget(Creature target, NHitInfo hitInfo)
        {
            if (Def.TargetType == TargetType.Self && target != context.Caster) return;
            else if (target == context.Caster) return;

            NDamageInfo dmgInfo = CalcSkillDmg(context.Caster, target);
            Log.InfoFormat("Skill[{0}].HitTarget[{1}] Dmg:{2} Crit:{3}", Def.Name, target.Name, dmgInfo.Dmg, dmgInfo.Crit);
            target.DealDamage(dmgInfo);
            hitInfo.Damages.Add(dmgInfo);

            AddBuff(TriggerType.SkillHit, target);
        }

        private NDamageInfo CalcSkillDmg(Creature caster, Creature target)
        {
            float dmg;
            if(Def.DmgType == DmgType.Physical)
            {
                dmg = caster.Attributes.AD * (1 - target.Attributes.DEF / (target.Attributes.DEF + 100)) * Def.DmgMultiplier;
            } else
            {
                dmg = caster.Attributes.AP * (1 - target.Attributes.MDEF / (target.Attributes.MDEF + 100)) * Def.DmgMultiplier;
            }

            bool ifCrit = CheckCritical(caster.Attributes.CRI);
            if (ifCrit) dmg *= 2;

            dmg *= (float)(MathUtil.Random.NextDouble() * 0.1f - 0.05f);

            NDamageInfo dmgInfo = new NDamageInfo();
            dmgInfo.entityId = target.entityId;
            dmgInfo.Dmg = Math.Max(1, (int)dmg);
            dmgInfo.Crit = ifCrit;
            return dmgInfo;
        }

        private bool CheckCritical(float cri)
        {
            return MathUtil.Random.NextDouble() < cri;
        }

        private void AddBuff(TriggerType trigger, Creature target)
        {
            if (Def.Buff == null || Def.Buff.Count == 0) return;
            foreach(var buffId in Def.Buff)
            {
                var buffDef = DataManager.Instance.Buffs[buffId];
                if (buffDef.Trigger != trigger) return;

                if(buffDef.TargetType == TargetType.Self)
                {
                    Owner.AddBuff(context, buffDef);
                } else if(buffDef.TargetType == TargetType.Single)
                {
                    target.AddBuff(context, buffDef);
                }
            }
        }

        internal void Update()
        {
            UpdateCD();
            if(Status == SkillStatus.Preparing)
            {
                UpdateCastPrep();
            } else if(Status == SkillStatus.Casting)
            {
                UpdateCastSkill();
            }
        }

        private void UpdateCastPrep()
        {
            if(prepTime < Def.CastTime)
            {
                prepTime += Time.deltaTime;
            } 
            else
            {
                Status = SkillStatus.Casting;
                Log.InfoFormat("Skill[{0}].UpdateCastPrep Over!", Def.Name);
            }
        }

        private void UpdateCastSkill()
        {
            castTime += Time.deltaTime;

            if(Def.Duration > 0)
            {
                if(castTime > Def.EffectInterval * (HitCount + 1))
                {
                    Hit();
                }

                if(castTime > Def.Duration)
                {
                    Status = SkillStatus.None;
                    Log.InfoFormat("Skill[{0}].UpdateCastSkill Over!", Def.Name);
                }
            } 
            else if(Def.HitTimes != null && Def.HitTimes.Count > 0)
            {
                if(HitCount < Def.HitTimes.Count)
                {
                    if(castTime > Def.HitTimes[HitCount])
                    {
                        Hit();
                    } 
                }
                else
                {
                    if(!Def.IfBullet)
                    {
                        Status = SkillStatus.None;
                        Log.InfoFormat("Skill[{0}].UpdateCastSkill Over!", Def.Name);
                    }
                }
            }

            if(Def.IfBullet)
            {
                bool finished = true;
                foreach(var bullet in bullets)
                {
                    bullet.Update();
                    if (!bullet.destroyed) finished = false;
                }

                if(finished && HitCount >= Def.HitTimes.Count)
                {
                    Status = SkillStatus.None;
                    Log.InfoFormat("Skill[{0}].UpdateCastSkill Over!", Def.Name);
                }
            }
        }

        private void UpdateCD()
        {
            if (cd > 0)
                cd -= Time.deltaTime;
            if (cd < 0)
                cd = 0;
        }
    }
}
