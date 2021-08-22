using Entities;
using System;
using System.Collections.Generic;
using Common.Battle;
using UnityEngine;

namespace Battle
{
    public class EffectManager
    {
        public Creature Owner;
        private Dictionary<BuffEffect, int> effects = new Dictionary<BuffEffect, int>();

        public EffectManager(Creature creature)
        {
            Owner = creature;
        }

        public void AddBuffEffect(BuffEffect effect)
        {
            Debug.LogFormat("AddBuffEffect: Target:[{0}] Effect:[{1}]", Owner.Name, effect);
            if (!effects.ContainsKey(effect))
                effects[effect] = 1;
            else
                effects[effect]++;
        }

        public void RemoveBuffEffect(BuffEffect effect)
        {
            Debug.LogFormat("RemoveBuffEffect: Target:[{0}] Effect:[{1}]", Owner.Name, effect);
            if (effects[effect] > 0)
                effects[effect]--;
        }

        public bool HasEffect(BuffEffect effect)
        {
            int count;
            if (effects.TryGetValue(effect, out count))
            {
                return count > 0;
            }
            return false;
        }
    }
}
