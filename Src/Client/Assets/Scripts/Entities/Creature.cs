using System;
using System.Collections.Generic;
using Battle;
using Common.Battle;
using Common.Data;
using Managers;
using SkillBridge.Message;
using UnityEngine;

namespace Entities
{
    public class Creature : Entity
    {
        public NCharacterInfo Info;

        public CharacterDefine Define;

        public Attributes Attributes;

        public SkillManager skillMgr;
        public BuffManager buffMgr;
        public EffectManager effectMgr;

        public Action<Buff,BuffAction> OnBuffChanged;

        private bool battleStat = false;
        public bool BattleStat
        {
            get { return battleStat; }
            set
            {
                battleStat = value;
                SetStandby(value);
            }
        }

        public int Id
        {
            get { return Info.Id; }
        }

        public Skill SkillBeingCasted = null;

        public string Name
        {
            get
            {
                if (Info.Type == CharacterType.Player)
                    return Info.Name;
                else
                    return Define.Name;
            }
        }

        public bool IsPlayer
        {
            get { return Info.Type == CharacterType.Player; }
        }

        public bool IsLocalPlayer 
        {
            get 
            {
                if (!IsPlayer) return false;
                return Id == Models.User.Instance.CurrentCharacterInfo.Id;
            }
        }

        public float DistanceTo(Creature target)
        {
            return Vector3Int.Distance(position, target.position);
        }

        public float DistanceTo(Vector3Int pos)
        {
            return Vector3Int.Distance(position, pos);
        }

        public Creature(NCharacterInfo info) : base(info.Entity)
        {
            Info = info;
            Define = DataManager.Instance.Characters[info.ConfigId];

            Attributes = new Attributes();
            Attributes.Init(Define, info.Level, GetEquips(), info.dynamicAttri);

            skillMgr = new SkillManager(this);
            buffMgr = new BuffManager(this);
            effectMgr = new EffectManager(this);
        }

        public void UpdateInfo(NCharacterInfo info)
        {
            SetEntityData(info.Entity);
            Info = info;
            Attributes.Init(Define, info.Level, GetEquips(), info.dynamicAttri);
            skillMgr.UpdateSkills();
        }

        public virtual List<EquipDefine> GetEquips()
        {
            return null;
        }

        public void MoveForward()
        {
            Debug.LogFormat("MoveForward");
            speed = Define.Speed;
        }

        public void MoveBack()
        {
            Debug.LogFormat("MoveBack");
            speed = -Define.Speed;
        }

        public void Stop()
        {
            Debug.LogFormat("Stop");
            speed = 0;
        }

        public void FaceTo(Vector3Int toFace)
        {
            SetDirection(GameObjectTool.WorldUnitToLogicInt(GameObjectTool.LogicUnitToWorld(toFace - position).normalized));
            UpdateEntityData();
            if (Controller != null)
                Controller.UpdateDirection();
        }

        public void SetDirection(Vector3Int direction)
        {
            this.direction = direction;
        }

        public void SetPosition(Vector3Int position)
        {
            this.position = position;
        }

        public void CastSkill(int skillId, Creature target, NVector3 position)
        {
            SetStandby(true);
            var skill = skillMgr.GetSkill(skillId);
            skill.CastEffect(target, position);
        }

        public void SetStandby(bool ifStandby)
        {
            if (Controller != null)
                Controller.SetStandby(ifStandby);
        }

        public void PlayAnim(string anim)
        {
            if (Controller != null)
                Controller.PlayAnim(anim);
        }

        public override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);

            skillMgr.OnUpdate(delta);
            buffMgr.OnUpdate(delta);
        }

        public void DealDamage(NDamageInfo damage, bool playHurt)
        {
            Debug.LogFormat("DeltDamage: Creature{0} DmgToken:{1}", entityId, damage.Dmg);
            Attributes.HP -= damage.Dmg;
            if(playHurt) PlayAnim("Hurt");

            if(Controller != null)
            {
                UIWorldElementManager.Instance.ShowPopUpText(PopUpType.Dmg, Controller.GetTransform().position + GetDmgPromptOffset(), - damage.Dmg, damage.Crit);
            }
        }

        public void InitSkillHit(NHitInfo hitInfo)
        {
            var skill = skillMgr.GetSkill(hitInfo.skillId);
            skill.InitHit(hitInfo);
        }

        public void HandleBuffAction(NBuffInfo buff)
        {
            switch(buff.Action)
            {
                case BuffAction.Add:
                    AddBuff(buff.Uid,buff.Tid,buff.casterId);
                    break;
                case BuffAction.Remove:
                    RemoveBuff(buff.Uid);
                    break;
                case BuffAction.Apply:
                    DealDamage(buff.Damage,false);
                    break;
            }
        }

        private void AddBuff(int uid, int tid, int casterId)
        {
            var buff = buffMgr.AddBuff(uid, tid, casterId);
            if (buff != null)
            {
                if (OnBuffChanged != null) OnBuffChanged(buff, BuffAction.Add);
            }
        }

        public void RemoveBuff(int uid)
        {
            var buff = buffMgr.RemoveBuff(uid);
            if(buff != null)
            {
                if (OnBuffChanged != null) OnBuffChanged(buff, BuffAction.Remove);
            }
        }

        public void AddBuffEffect(BuffEffect effect)
        {
            effectMgr.AddBuffEffect(effect);
        }

        public void RemoveBuffEffect(BuffEffect effect)
        {
            effectMgr.RemoveBuffEffect(effect);
        }

        public void PlayEffect(EffectType type, string name, Creature target, float duration = 0)
        {
            if (string.IsNullOrEmpty(name)) return;
            if (Controller != null)
                Controller.PlayEffect(type,name,target,duration);
        }

        public void PlayEffect(EffectType type, string name, NVector3 pos)
        {
            if (string.IsNullOrEmpty(name)) return;
            if (Controller != null)
                Controller.PlayEffect(type, name, pos, 0);
        }

        public Vector3 GetHitOffset()
        {
            return new Vector3(0, Define.Height * 0.8f, 0);
        }

        public Vector3 GetDmgPromptOffset()
        {
            return new Vector3(0, Define.Height, 0);
        }
    }
}
