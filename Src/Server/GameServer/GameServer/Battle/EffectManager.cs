using Common;
using Common.Battle;
using GameServer.Entities;
using System;
using System.Collections.Generic;

namespace GameServer.Battle
{
    class EffectManager
    {
        public Creature Owner;
        private Dictionary<BuffEffect, int> effects = new Dictionary<BuffEffect, int>();

        public EffectManager(Creature creature)
        {
            Owner = creature;
        }

        public void AddBuffEffect(BuffEffect effect)
        {
            Log.InfoFormat("AddBuffEffect: Target:[{0}] Effect:[{1}]", Owner.Name, effect);
            if (!effects.ContainsKey(effect))
                effects[effect] = 1;
            else
                effects[effect] ++;
        }

        public void RemoveBuffEffect(BuffEffect effect)
        {
            Log.InfoFormat("RemoveBuffEffect: Target:[{0}] Effect:[{1}]", Owner.Name, effect);
            if (effects[effect] > 0)
                effects[effect]--;
        }

        public bool HasEffect(BuffEffect effect)
        {
            if(effects.TryGetValue(effect, out int count))
            {
                return count > 0;
            }
            return false;
        }
    }
}
