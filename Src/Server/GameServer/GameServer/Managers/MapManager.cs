using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Models;

namespace GameServer.Managers
{
    class MapManager : Singleton<MapManager>
    {
        Dictionary<int, Dictionary<int,Map>> Maps = new Dictionary<int, Dictionary<int, Map>>();

        public void Init()
        {
            foreach (var mapdefine in DataManager.Instance.Maps.Values)
            {
                int instanceCount = 1;
                if (mapdefine.Type == Common.Data.MapType.Arena)
                    instanceCount = ArenaManager.MaxInstance;
                if (mapdefine.Type == Common.Data.MapType.Story)
                    instanceCount = StoryManager.MaxInstance;
                Log.InfoFormat("MapManager.Init > Map:{0}:{1}", mapdefine.ID, mapdefine.Name);
                Maps[mapdefine.ID] = new Dictionary<int, Map>();
                for(int i = 0; i < instanceCount; i++)
                {
                    Maps[mapdefine.ID][i] = new Map(mapdefine,i);
                }
            }
        }

        public Map this[int key]
        {
            get
            {
                return Maps[key][0];
            }
        }

        public Map GetInstance(int arenaMapId, int idx)
        {
            return Maps[arenaMapId][idx];
        }

        public void Update()
        {
            foreach(var maps in Maps.Values)
            {
                foreach(var instance in maps.Values)
                {
                    instance.Update();
                }
            }
        }
    }
}
