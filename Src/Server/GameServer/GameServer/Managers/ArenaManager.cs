using System;
using System.Collections.Generic;
using Common;
using GameServer.Models;
using GameServer.Services;
using Network;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class ArenaManager : Singleton<ArenaManager>
    {
        public const int ArenaMapId = 5;
        public const int MaxInstance = 100;

        Queue<int> InstanceIdxes = new Queue<int>();
        Arena[] Arenas = new Arena[MaxInstance];

        public void Init()
        {
            for(int i = 0; i < MaxInstance; i++)
            {
                InstanceIdxes.Enqueue(i);
            }
        }

        public Arena NewArena(ArenaInfo info, NetConnection<NetSession> blue, NetConnection<NetSession> red)
        {
            var idx = InstanceIdxes.Dequeue();
            var map = MapManager.Instance.GetInstance(ArenaMapId,idx);

            Arena curr = (Arenas[idx] == null) ? new Arena(map, info, blue, red) : Arenas[idx];
            curr.Init(map, info, blue, red);
            Arenas[idx] = curr;
            curr.PlayersIn();
            return curr;
        }

        public void RemoveArena(int arenaId)
        {
            Arena arena = Arenas[arenaId];
            arena.Clear();
            InstanceIdxes.Enqueue(arenaId);
        }

        internal void Update()
        {
            for(int i = 0; i < Arenas.Length; i++)
            {
                if(Arenas[i] != null)
                {
                    Arenas[i].Update();
                }
            }
        }

        public Arena GetArena(int arenaId)
        {
            return Arenas[arenaId];
        }
    }
}
