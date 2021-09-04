using Battle;
using Entities;
using Models;
using SkillBridge.Message;
using System;
using UnityEngine.Events;
using Common.Battle;
using Services;

namespace Managers
{
    class BattleManager : Singleton<BattleManager>
    {
        public delegate void TargetChangedHandler(Creature target);
        public event TargetChangedHandler OnTargetChanged;

        private Creature target;
        public Creature Target
        {
            get { return target; }
            set { SetTarget(value); }
        }

        private NVector3 effectLoc;
        public NVector3 EffectLoc
        {
            get { return effectLoc; }
            set { SetEffectLoc(value); }
        }

        public void SetTarget(Creature t)
        {
            if (OnTargetChanged != null)
                OnTargetChanged(t);
            target = t;
        }

        public void SetEffectLoc(NVector3 loc)
        {
            effectLoc = loc;
        }

        public void CastSkill(Skill skill, NVector3 LocSelected = null)
        {
            var tarId = target == null ? 0 : target.entityId;
            BattleService.Instance.SendCastSkill(skill.Def.ID, skill.Owner.entityId, tarId, effectLoc);
        }
    }
}
