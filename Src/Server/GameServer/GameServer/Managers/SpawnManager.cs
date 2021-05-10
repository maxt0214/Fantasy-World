using Common;
using System;
using System.Collections.Generic;
using SkillBridge.Message;
using GameServer.Entities;
using GameServer.Models;

namespace GameServer.Managers
{
    class SpawnManager
    {
        private List<Spawner> Rules = new List<Spawner>();

        private Map map;

        public void Init(Map map)
        {
            this.map = map;
            if(DataManager.Instance.SpawnRules.ContainsKey(map.ID))
            {
                foreach(var def in DataManager.Instance.SpawnRules[map.ID].Values)
                {
                    Rules.Add(new Spawner(def,this.map));
                }
            }
        }

        public void Update()
        {
            if (Rules.Count == 0)
                return;

            for(int i = 0; i < Rules.Count; i++)
            {
                Rules[i].Update();
            }
        }
    }
}
