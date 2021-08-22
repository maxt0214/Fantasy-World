using Common.Data;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Battle
{
    public class BuffManager
    {
        public Creature Owner;
        internal Dictionary<int,Buff> buffs = new Dictionary<int, Buff>();

        public BuffManager(Creature creature)
        {
            Owner = creature;
        }

        public Buff AddBuff(int uid, int tid, int casterId)
        {
            BuffDefine define;
            if(DataManager.Instance.Buffs.TryGetValue(tid,out define))
            {

                Buff buff = new Buff(uid, Owner, define, casterId);
                buffs[uid] = buff;
                return buff;
            }
            return null;
        }

        public Buff RemoveBuff(int uid)
        {
            Buff buff;
            if(buffs.TryGetValue(uid, out buff))
            {
                buff.OnRemoved();
                buffs.Remove(uid);
                return buff;
            }
            return null;
        }

        internal void OnUpdate(float delta)
        {
            List<int> toRemove = new List<int>();
            foreach(var kv in buffs)
            {
                kv.Value.OnUpdate(delta);
                if (kv.Value.Finished)
                    toRemove.Add(kv.Key);
            }

            foreach(var key in toRemove)
            {
                Owner.RemoveBuff(key);
            }
        }
    }
}
