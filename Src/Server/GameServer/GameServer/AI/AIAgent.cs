using Common.Battle;
using GameServer.Battle;
using GameServer.Entities;
using GameServer.Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;

namespace GameServer.AI
{
    class AIAgent
    {
        Monster Owner;

        private AIBase ai;

        public AIAgent(Monster monster)
        {
            Owner = monster;
            var ait = monster.Define.AI;
            if (ait == null) ait = AIPassiveMonster.ID;
            switch(ait)
            {
                case AIPassiveMonster.ID:
                    ai = new AIPassiveMonster(Owner);
                    break;
                case AIBoss.ID:
                    ai = new AIBoss(Owner);
                    break;
            }

        }

        public void Init()
        {

        }

        public void Update()
        {
            if(ai != null)
            {
                ai.Update();
            }
        }

        public void OnDamaged(NDamageInfo damage, Creature source)
        {
            if(ai != null)
            {
                ai.OnDamaged(damage, source);
            }
        }
    }
}
