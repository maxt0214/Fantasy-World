using GameServer.Core;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Battle
{
    class BattleContext
    {
        public Battle Battle;
        public Creature Caster;
        public Creature Target;
        public Vector3Int Position;
        public NCastSkillInfo CastingSkill;
        public SkillResult Result;

        public BattleContext(Battle battle)
        {
            Battle = battle;
        }
    }
}
