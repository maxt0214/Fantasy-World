using GameServer.Entities;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;

namespace GameServer.Battle
{
    class SkillManager
    {
        private Creature Owner;

        public List<Skill> Skills { get; private set; }
        public List<NSkillInfo> Infos { get; private set; }

        public Skill atk { get; private set; }

        public SkillManager(Creature owner)
        {
            Owner = owner;
            Skills = new List<Skill>();
            Infos = new List<NSkillInfo>();

            InitSkills();
        }

        private void InitSkills()
        {
            Skills.Clear();
            Infos.Clear();

            if (!DataManager.Instance.Skills.ContainsKey(Owner.Define.TID))
                return;

            foreach(var kv in DataManager.Instance.Skills[Owner.Define.TID])
            {
                var info = new NSkillInfo();
                info.Id = kv.Key;
                info.Lv = 1;
                Infos.Add(info);
                Skill skill = new Skill(info, Owner);
                if(kv.Value.SkillType == Common.Battle.SkillType.Normal)
                {
                    atk = skill;
                }
                AddSkill(skill);
            }
        }

        private void AddSkill(Skill skill)
        {
            Skills.Add(skill);
        }

        public bool ContainSkill(int sid)
        {
            foreach(var skill in Infos)
            {
                if (skill.Id == sid)
                    return true;
            }
            return false;
        }

        public Skill GetSkill(int skillId)
        {
            foreach(var skill in Skills)
            {
                if (skill.Info.Id == skillId)
                    return skill;
            }
            return null;
        }

        internal void Update()
        {
            for(int i = 0; i < Skills.Count; i++)
            {
                Skills[i].Update();
            }
        }
    }
}
