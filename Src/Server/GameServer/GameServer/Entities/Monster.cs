using Common.Battle;
using GameServer.AI;
using GameServer.Battle;
using GameServer.Core;
using GameServer.Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;

namespace GameServer.Entities
{
    class Monster : Creature
    {
        private AIAgent AI;
        private Vector3Int destiny;
        private Vector3 destination;

        public Monster(int tid, int level, Vector3Int pos, Vector3Int dir) : base(CharacterType.Monster, tid, level, pos, dir)
        {
            AI = new AIAgent(this);
        }

        public override void OnEnteredMap(Map map)
        {
            base.OnEnteredMap(map);
            AI.Init();
        }

        public override void Update()
        {
            base.Update();
            UpdatePosition();
            AI.Update();
        }

        public Skill FindSkill(BattleContext context, SkillType type)
        {
            Skill castableSkill = null;
            foreach (var skill in SkillMgr.Skills)
            {
                if ((skill.Def.SkillType & type) != skill.Def.SkillType) continue;

                var res = skill.Castable(context);
                if (res == SkillResult.Casting)
                    return null;
                if (res == SkillResult.Valid)
                    castableSkill = skill;
            }
            return castableSkill;
        }

        internal override void OnDamaged(NDamageInfo damage, Creature source)
        {
            if (AI != null)
                AI.OnDamaged(damage,source);
        }

        public void MoveTo(Vector3Int position)
        {
            if(MotionStat == CharacterState.Idle)
            {
                MotionStat = CharacterState.Move;
            }

            if(destiny != position)
            {
                destiny = position;
                destination = Position;
                var dist = (destiny - Position);

                Direction = dist.normalized;

                Speed = Define.Speed;

                NEntitySync sync = new NEntitySync();
                sync.Entity = EntityData;
                sync.Event = EntityEvent.MoveFwd;
                sync.Id = entityId;
                Map.UpdateEntity(sync);
            }
        }

        private void UpdatePosition()
        {
            if (MotionStat == CharacterState.Move)
            {
                if (DistanceTo(destiny) < 50f)
                {
                    Stop();
                }

                if (Speed > 0)
                {
                    Vector3 dir = Direction;
                    destination += dir * Speed * Time.deltaTime / 100f;
                    Position = destination;
                }
            }
        }

        public void Stop()
        {
            MotionStat = CharacterState.Idle;
            destiny = Vector3Int.zero;
            Speed = 0;

            NEntitySync sync = new NEntitySync();
            sync.Entity = EntityData;
            sync.Event = EntityEvent.Idle;
            sync.Id = entityId;

            Map.UpdateEntity(sync);
        }
    }
}
