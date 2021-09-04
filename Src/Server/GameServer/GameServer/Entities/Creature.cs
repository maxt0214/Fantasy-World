using Common.Battle;
using Common.Data;
using GameServer.Battle;
using GameServer.Core;
using GameServer.Managers;
using GameServer.Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;

namespace GameServer.Entities
{
    class Creature : Entity
    {

        public int Id { get; set; }
        public string Name { get { return Info.Name; } }

        public NCharacterInfo Info;
        public CharacterDefine Define;

        public Attributes Attributes;
        public SkillManager SkillMgr;
        public BuffManager BuffMgr;
        public EffectManager EffectMgr;

        public delegate void CreatureDeadHandler(int eid);
        public CreatureDeadHandler OnDead;

        public delegate void CreaturekillHandler(CharacterDefine def);
        public CreaturekillHandler OnKill;

        public CreatureState BattleStat;
        public CharacterState MotionStat;

        public Map Map;

        public Creature(CharacterType type, int configId, int level, Vector3Int pos, Vector3Int dir) :
           base(pos, dir)
        {
            Info = new NCharacterInfo();
            Info.Type = type;
            Info.Level = level;
            Info.ConfigId = configId;
            Define = DataManager.Instance.Characters[Info.ConfigId];
            Info.Entity = EntityData;
            Info.EntityId = entityId;
            Info.Name = Define.Name;

            InitSkills();
            InitBuffs();

            Attributes = new Attributes();
            Attributes.Init(Define,level,GetEquips(),Info.dynamicAttri);
            Info.dynamicAttri = Attributes.DynamicAttri;
        }

        public float DistanceTo(Creature target)
        {
            return Vector3Int.Distance(Position, target.Position);
        }

        public float DistanceTo(Vector3Int position)
        {
            return Vector3Int.Distance(Position, position);
        }

        private void InitSkills()
        {
            SkillMgr = new SkillManager(this);
            Info.Skills.AddRange(SkillMgr.Infos);
        }

        private void InitBuffs()
        {
            BuffMgr = new BuffManager(this);
            EffectMgr = new EffectManager(this);
        }

        public void AddBuff(BattleContext context, BuffDefine buffDef)
        {
            BuffMgr.AddBuff(context, buffDef);
        }

        public virtual List<EquipDefine> GetEquips()
        {
            return null;
        }

        public override void Update()
        {
            SkillMgr.Update();
            BuffMgr.Update();
        }

        public bool IsDead()
        {
            return Attributes.HP <= 0;
        }

        public void CastSkill(BattleContext context, int skillId)
        {
            Skill skill = SkillMgr.GetSkill(skillId);
            if (skill == null)
                context.Result = SkillResult.InvalidSkill;
            else
                context.Result = skill.Cast(context);

            if(context.Result == SkillResult.Valid)
            {
                BattleStat = CreatureState.InBattle;
            }

            if(context.CastingSkill == null) //Monster
            {
                if(context.Result == SkillResult.Valid)
                {
                    context.CastingSkill = new NCastSkillInfo()
                    {
                        casterId = entityId,
                        targetId = context.Target.entityId,
                        skillId = skill.Def.ID,
                        Position = new NVector3(),
                        Result = context.Result,
                    };
                    context.Battle.AddCastInfo(context.CastingSkill);
                }
            } else //Players
            {
                context.CastingSkill.Result = context.Result;
                context.Battle.AddCastInfo(context.CastingSkill);
            }
        }

        public void DealDamage(NDamageInfo damage, Creature source)
        {
            BattleStat = CreatureState.InBattle;
            Attributes.HP -= damage.Dmg;
            if(IsDead())
            {
                OnDead?.Invoke(entityId);
                damage.DeadAfterDmg = true;
            }
            OnDamaged(damage,source);
        }

        internal virtual void OnDamaged(NDamageInfo damage, Creature source)
        {
        }

        public virtual void OnEnteredMap(Map map)
        {
            Map = map;
        }

        public virtual void OnLeftMap(Map map)
        {
            Map = null;
            map.Battle.LeaveBattle(this);
        }

        public void OnKilled(Creature target)
        {
            OnKill?.Invoke(target.Define);
        }
    }
}
