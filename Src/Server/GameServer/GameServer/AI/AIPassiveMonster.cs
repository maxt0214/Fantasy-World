using GameServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.AI
{
    class AIPassiveMonster : AIBase
    {
        public const string ID = "Passive";

        public AIPassiveMonster(Monster mon) : base(mon)
        {
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
