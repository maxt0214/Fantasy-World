using Common;
using System;
using System.Collections.Generic;
using SkillBridge.Message;
using GameServer.Entities;
using GameServer.Models;

namespace GameServer.Managers
{
    class MonsterManager
    {
        private Map map;

        public Dictionary<int, Monster> monsters = new Dictionary<int, Monster>();

        public void Init(Map map)
        {
            this.map = map;
        }

        internal Monster Create(int monId, int spawnLevel, NVector3 spawnPos, NVector3 spawnDir)
        {
            Monster monster = new Monster(monId, spawnLevel, spawnPos, spawnDir);
            EntityManager.Instance.AddEntity(map.ID, monster);
            monster.Id = monster.entityId;
            monster.Info.EntityId = monster.entityId;
            monster.Info.mapId = map.ID;
            monsters[monster.Id] = monster;
            map.MonsterEnter(monster);
            return monster;
        }
    }
}
