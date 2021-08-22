using System;
using System.Collections.Generic;
using Entities;
using UnityEngine;

namespace Battle
{
    public class SkillManager
    {
        private Creature Owner;
        public List<Skill> skills { get; private set; }

        public SkillManager(Creature owner)
        {
            Owner = owner;
            skills = new List<Skill>();
            InitSkills();
        }

        private void InitSkills()
        {
            skills.Clear();
            foreach(var skillInfo in Owner.Info.Skills)
            {
                Skill skill = new Skill(skillInfo, Owner);
                AddSkill(skill);
            }
        }

        public void UpdateSkills()
        {
            foreach(var skillInfo in Owner.Info.Skills)
            {
                Skill skill = GetSkill(skillInfo.Id);
                if (skill != null)
                    skill.Info = skillInfo;
                else
                    AddSkill(skill);
            }
        }

        private void AddSkill(Skill skill)
        {
            skills.Add(skill);
        }

        public Skill GetSkill(int skillId)
        {
            foreach(var skill in skills)
            {
                if (skill.Def.ID == skillId)
                    return skill;
            }
            return null;
        }

        public void OnUpdate(float delta)
        {
            for(int i = 0; i < skills.Count; i ++)
            {
                skills[i].OnUpdate(delta);
            }
        }
    }
}
