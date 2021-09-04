using Common.Battle;
using GameServer.Battle;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;

namespace GameServer.AI
{
    class AIBase
    {
        protected Monster Owner;
        protected Creature Target;

        private Skill atk;

        public AIBase(Monster mon)
        {
            Owner = mon;
            atk = Owner.SkillMgr.atk;
        }

        public virtual void Update()
        {
            if (Owner.BattleStat == CreatureState.InBattle)
            {
                MakeDecision();
            }
        }

        protected void MakeDecision()
        {
            if(Target == null)
            {
                Owner.BattleStat = CreatureState.Idle;
                return;
            }

            if (!TryCastSkill())
            {
                if(!TryNormalAtk())
                {
                    TraceTarget();
                }
            }
        }

        private bool TryCastSkill()
        {
            if(Target != null)
            {
                BattleContext context = new BattleContext(Owner.Map.Battle)
                {
                    Target = Target,
                    Caster = this.Owner
                };

                Skill skill = Owner.FindSkill(context,SkillType.Skill);
                if (skill != null)
                {
                    Owner.CastSkill(context, skill.Def.ID);
                    return context.Result == SkillResult.Valid;
                }
            }

            return false;
        }

        private bool TryNormalAtk()
        {
            if (Target != null)
            {
                BattleContext context = new BattleContext(Owner.Map.Battle)
                {
                    Target = Target,
                    Caster = this.Owner
                };

                var res = atk.Castable(context);
                if (res == SkillResult.Valid)
                {
                    Owner.CastSkill(context, atk.Def.ID);
                    return true;
                }

                if(res == SkillResult.OutOfRange)
                {
                    return false;
                }
            }

            return true;
        }

        private void TraceTarget()
        {
            if (Target == null) return;

            int dis = (int)Owner.DistanceTo(Target);
            if(dis > atk.Def.Range - 50)
            {
                Owner.MoveTo(Target.Position);
            } else
            {
                Owner.Stop();
            }
        }

        public void OnDamaged(NDamageInfo damage, Creature source)
        {
            if (source != null)
            {
                Target = source;
            }
        }
    }
}
