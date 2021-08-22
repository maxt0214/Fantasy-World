using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Entities
{
    public interface IEntityController
    {
        void PlayAnim(string name);
        void SetStandby(bool ifStandby);
        void UpdateDirection();
        void PlayEffect(EffectType type, string name, Creature target, float duration);
        Transform GetTransform();
    }
}
