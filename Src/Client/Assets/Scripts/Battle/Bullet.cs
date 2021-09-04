using System;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    class Bullet
    {
        private Skill skill;
        private int hit = 0;
        public bool destroyed = false;

        private float shotTime = 0;
        public float duration = 0;

        public Bullet(Skill skl)
        {
            skill = skl;
            var target = skill.Target;
            hit = skill.HitCount;
            float distance = skill.Owner.DistanceTo(target);
            duration = distance / skill.Def.BulletSpd;
        }

        public void Update()
        {
            if (destroyed) return;

            shotTime += Time.deltaTime;
            if(shotTime > duration)
            {
                skill.DealHitDamage(hit);
                Destroy();
            }
        }

        public void Destroy()
        {
            destroyed = true;
        }
    }
}
