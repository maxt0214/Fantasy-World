using Common.Data;
using GameServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Battle
{
    class BuffManager
    {
        public Creature Owner;
        private List<Buff> buffs = new List<Buff>();

        private int idx = 1;
        public int BuffID
        {
            get { return idx++; }
        }

        public BuffManager(Creature creature)
        {
            Owner = creature;
        }

        public void AddBuff(BattleContext context, BuffDefine def)
        {
            Buff buff = new Buff(BuffID, Owner, def, context);
            buffs.Add(buff);
        }

        internal void Update()
        {
            for(int i = 0; i < buffs.Count; i++)
            {
                if(!buffs[i].Finished)
                {
                    buffs[i].Update();
                }
            }
            buffs.RemoveAll(b => b.Finished);
        }
    }
}
