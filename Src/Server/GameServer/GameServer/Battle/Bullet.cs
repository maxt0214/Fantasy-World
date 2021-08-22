using Common;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Battle
{
    class Bullet
    {
        private Skill skill;
        private bool timeMode = true;
        public bool destroyed = false;
        public NHitInfo hitInfo;

        private float shotTime = 0;
        private float duration = 0;

        public Bullet(Skill skl, Creature target, NHitInfo hitIn)
        {
            skill = skl;
            hitInfo = hitIn;

            var distance = skill.Owner.DistanceTo(target);
            if(timeMode)
            {
                duration = distance / skill.Def.BulletSpd;
                Log.InfoFormat("Bullet[{0}] Resource[{1}] Target:{2} Distance:{3} BulletSpd:{4} TimeOfHit:{5}", skill.Def.Name, skill.Def.BulletRes, target.Name, distance, skill.Def.BulletSpd, duration);
            }
        }

        public void Update()
        {
            if (destroyed) return;

            if(timeMode)
            {
                UpdateTime();
            } else
            {
                UpdatePos();
            }
        }

        private void UpdateTime()
        {
            shotTime += Time.deltaTime;
            if(shotTime > duration)
            {
                hitInfo.ifBullet = true;
                skill.Hit(hitInfo);
                destroyed = true;
            }
        }

        private void UpdatePos()
        {

        }
    }
}
