using System;
using System.Collections.Generic;
using Common.Battle;
using Common.Data;
using Managers;
using SkillBridge.Message;
using UnityEngine;

namespace Entities
{
    public class Character : Creature
    {
        public Character(NCharacterInfo info) : base(info)
        {

        }

        public override List<EquipDefine> GetEquips()
        {
            return EquipManager.Instance.GetEquips();
        }
    }
}
