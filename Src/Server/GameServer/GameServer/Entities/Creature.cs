using Common.Battle;
using Common.Data;
using GameServer.Battle;
using GameServer.Core;
using GameServer.Managers;
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

        public bool ifDead = false;

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
        }

        public void DealDamage(NDamageInfo damage)
        {
            Attributes.HP -= damage.Dmg;
            if(IsDead())
            {
                ifDead = true;
                damage.DeadAfterDmg = true;
            }
        }
    }
}
