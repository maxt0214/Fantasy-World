using Common.Battle;
using Common.Data;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Battle
{
    public class Buff
    {
        public int ID;
        public Creature Owner;
        public BuffDefine Def;
        public int CasterId;

        private float time = 0;

        public bool Finished = false;

        public Buff(int uid, Creature owner, BuffDefine define, int casterId)
        {
            ID = uid;
            Owner = owner;
            Def = define;
            CasterId = casterId;
            OnAdded();
        }

        private void OnAdded()
        {
            Debug.LogFormat("BuffOnAdded: Buff[{0}] Owner[{1}]", Def.Name, Owner.Name);
            if (Def.Effect != BuffEffect.None)
            {
                Owner.AddBuffEffect(Def.Effect);
            }
            AddAttri();
        }

        public void OnRemoved()
        {
            Debug.LogFormat("BuffOnRemoved: Buff[{0}] Owner[{1}]", Def.Name, Owner.Name);
            RemoveAttri();
            Finished = true;
            if (Def.Effect != BuffEffect.None)
            {
                Owner.RemoveBuffEffect(Def.Effect);
            }
        }

        private void AddAttri()
        {
            if (Def.DEFRatio != 0)
            {
                Owner.Attributes.Buff.DEF += Owner.Attributes.Buff.DEF * Def.DEFRatio;
                Owner.Attributes.SetFinalAttri();
            }
        }

        private void RemoveAttri()
        {
            if (Def.DEFRatio != 0)
            {
                Owner.Attributes.Buff.DEF -= Owner.Attributes.Buff.DEF * Def.DEFRatio;
                Owner.Attributes.SetFinalAttri();
            }
        }

        internal void OnUpdate(float delta)
        {
            if (Finished) return;

            time += Time.deltaTime;

            if(time > Def.Duration)
            {
                OnRemoved();
            }
        }
    }
}
