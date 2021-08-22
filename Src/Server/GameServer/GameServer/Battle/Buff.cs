using Common;
using Common.Battle;
using Common.Data;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;

namespace GameServer.Battle
{
    class Buff
    {
        public int ID;
        public Creature Owner;
        public BuffDefine Def;
        public BattleContext Context;

        private float time = 0;
        private int count = 0;

        public bool Finished { get; private set; } = false;

        public Buff(int buffID, Creature owner, BuffDefine def, BattleContext ctx)
        {
            ID = buffID;
            Owner = owner;
            Def = def;
            Context = ctx;

            OnAdded();
        }

        private void OnAdded()
        {
            if(Def.Effect != BuffEffect.None)
            {
                Owner.EffectMgr.AddBuffEffect(Def.Effect);
            }

            AddAttri();

            NBuffInfo buffInfo = new NBuffInfo()
            {
                Uid = ID,
                Tid = Def.ID,
                casterId = Context.Caster.entityId,
                ownerId = Owner.entityId,
                Action = BuffAction.Add,
            };
            Context.Battle.AddBuffAction(buffInfo);
        }

        private void OnRemoved()
        {
            RemoveAttri();
            Finished = true;
            if (Def.Effect != BuffEffect.None)
            {
                Owner.EffectMgr.RemoveBuffEffect(Def.Effect);
            }

            NBuffInfo buffInfo = new NBuffInfo()
            {
                Uid = ID,
                Tid = Def.ID,
                casterId = Context.Caster.entityId,
                ownerId = Owner.entityId,
                Action = BuffAction.Remove,
            };
            Context.Battle.AddBuffAction(buffInfo);
        }

        private void AddAttri()
        {
            if(Def.DEFRatio != 0)
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

        internal void Update()
        {
            if (Finished) return;
            time += Time.deltaTime;

            if(Def.Interval > 0)
            {
                if(time > Def.Interval * (count + 1))
                {
                    ApplyBuff();
                }
            }

            if(time > Def.Duration)
            {
                OnRemoved();
            }
        }

        private void ApplyBuff()
        {
            count++;

            NDamageInfo dmgInfo = CalcBuffDmg(Context.Caster);
            Log.InfoFormat("Buff[{0}].AffectedTarget[{1}] Dmg:{2} Crit:{3}", Def.Name, Owner.Name, dmgInfo.Dmg, dmgInfo.Crit);
            Owner.DealDamage(dmgInfo);

            NBuffInfo buffInfo = new NBuffInfo()
            {
                Uid = ID,
                Tid = Def.ID,
                casterId = Context.Caster.entityId,
                ownerId = Owner.entityId,
                Action = BuffAction.Apply,
                Damage = dmgInfo
            };
            Context.Battle.AddBuffAction(buffInfo);
        }

        private NDamageInfo CalcBuffDmg(Creature caster)
        {
            float ad = Def.AD + caster.Define.AD * Def.ADFactor;
            float ap = Def.AP + caster.Define.AP * Def.APFactor;

            float adDmg = ad * (1 - Owner.Attributes.DEF / (Owner.Attributes.DEF + 100));
            float apDmg = ap * (1 - Owner.Attributes.MDEF / (Owner.Attributes.MDEF + 100));

            float final = adDmg + apDmg;

            NDamageInfo dmgInfo = new NDamageInfo();
            dmgInfo.entityId = Owner.entityId;
            dmgInfo.Dmg = Math.Max(1, (int)final);
            return dmgInfo;
        }
    }
}
