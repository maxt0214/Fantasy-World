using Common;
using System;
using System.Collections.Generic;
using SkillBridge.Message;
using GameServer.Entities;
using GameServer.Models;
using Common.Data;

namespace GameServer.Managers
{
    class Spawner
    {
        private SpawnRuleDefine define;
        private Map map;

        private float spawnCycleStartTime = 0f; //Start time of a new spawn cycle. Either monster dead or never spawned before

        private bool spawned = false;

        private SpawnPointDefine spawnPoint = null;

        public Spawner(SpawnRuleDefine def, Map map)
        {
            define = def;
            this.map = map;

            if(DataManager.Instance.SpawnPoints.ContainsKey(this.map.ID))
            {
                if(DataManager.Instance.SpawnPoints[this.map.ID].ContainsKey(define.SpawnPoint))
                {
                    spawnPoint = DataManager.Instance.SpawnPoints[this.map.ID][define.SpawnPoint];
                } else
                {
                    Log.ErrorFormat("SpawnRule[{0}] SpawnPoint[{1}] does not exist!", define.ID, define.SpawnPoint);
                }
            }
        }

        public void Update()
        {
            if(AbleToSpawn())
            {
                Spawn();
            }
        }

        private bool AbleToSpawn()
        {
            if (spawned) return false;
            if (spawnCycleStartTime + define.SpawnPeriod > Time.time) return false;

            return true;
        }

        private void Spawn()
        {
            spawned = true;
            Log.InfoFormat("Map[{0}] SpawnRule[{1}: Mon:{2}, Lv:{3}] SpawnPoint[{4}]", map.ID, define.ID, define.SpawnMonID, define.SpawnLevel, spawnPoint.ID);
            map.monsterManager.Create(define.SpawnMonID,define.SpawnLevel,spawnPoint.Position,spawnPoint.Direction);
        }
    }
}
