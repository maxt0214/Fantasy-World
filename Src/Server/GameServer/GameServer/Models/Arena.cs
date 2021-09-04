using Common;
using Common.Data;
using GameServer.Core;
using GameServer.Managers;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;

namespace GameServer.Models
{
    class Arena
    {
        public const float READY_COUNT_DOWN = 11f;
        public const float ROUND_TIME = 60f;
        public const float SETTLING_TIME = 5f;

        public Map Map;
        public ArenaInfo Info;
        public NetConnection<NetSession> Blue;
        public NetConnection<NetSession> Red;

        private Map sourceRedMap;
        private Map sourceBlueMap;
        private Vector3Int redPos;
        private Vector3Int redDir;
        private Vector3Int bluePos;
        private Vector3Int blueDir;

        private int redLoc = 9;
        private int blueLoc = 10;

        private bool redReady = false;
        private bool blueReady = false;

        private float timer = 0f;

        public bool Ready
        {
            get { return redReady && blueReady; }
        }

        public ArenaStatus ArenaStat;
        public ArenaRoundStatus RoundStat;
        public int Round { get; internal set; }

        public Arena(Map map, ArenaInfo info, NetConnection<NetSession> blue, NetConnection<NetSession> red)
        {
            Init(map, info, blue, red);
        }

        public void Init(Map map, ArenaInfo info, NetConnection<NetSession> blue, NetConnection<NetSession> red)
        {
            Map = map;
            Info = info;
            Blue = blue;
            Red = red;
            Info.arenaId = Map.InstanceID;
            ArenaStat = ArenaStatus.Waiting;
            RoundStat = ArenaRoundStatus.None;
        }

        public void PlayersIn()
        {
            bluePos = Blue.Session.Character.Position;
            blueDir = Blue.Session.Character.Direction;
            redPos = Red.Session.Character.Position;
            redDir = Red.Session.Character.Direction;

            sourceBlueMap = PlayerLeaveMap(Blue);
            sourceRedMap = PlayerLeaveMap(Red);

            SendPlayersInArena();
            Red.Session.Character.OnLeft += OnOnePartyLeftGame;
            Blue.Session.Character.OnLeft += OnOnePartyLeftGame;
            Red.Session.Character.OnDead += OnOnePartyDown;
            Blue.Session.Character.OnDead += OnOnePartyDown;
        }

        public void Clear()
        {
            PlayersOut();

            Map = null;
            Info = null;
            Blue = null;
            Red = null;
            ArenaStat = ArenaStatus.None;
            RoundStat = ArenaRoundStatus.None;
        }

        public void PlayersOut()
        {
            Red.Session.Character.OnDead -= OnOnePartyDown;
            Blue.Session.Character.OnDead -= OnOnePartyDown;
            Red.Session.Character.OnLeft -= OnOnePartyLeftGame;
            Blue.Session.Character.OnLeft -= OnOnePartyLeftGame;
            PlayerLeaveMap(Blue);
            PlayerLeaveMap(Red);

            SendPlayersBack();
        }

        private Map PlayerLeaveMap(NetConnection<NetSession> player)
        {
            var currMap = MapManager.Instance[player.Session.Character.Info.mapId];
            currMap.CharacterLeave(player.Session.Character);
            EntityManager.Instance.RemoveEntityInMap(currMap.ID,currMap.InstanceID,player.Session.Character);
            return currMap;
        }

        private void SendPlayersInArena()
        {
            TeleporterDefine blueLoc = DataManager.Instance.Teleporters[this.blueLoc];
            Blue.Session.Character.Position = blueLoc.Position;
            Blue.Session.Character.Direction = blueLoc.Direction;

            TeleporterDefine redLoc = DataManager.Instance.Teleporters[this.redLoc];
            Red.Session.Character.Position = redLoc.Position;
            Red.Session.Character.Direction = redLoc.Direction;

            //Add two parties to the map
            Map.AddCharaToMap(Red, Red.Session.Character);
            Map.AddCharaToMap(Blue, Blue.Session.Character);
            //Send messages about characters getting in
            Map.CharacterEnter(Red, Red.Session.Character);
            Map.CharacterEnter(Blue, Blue.Session.Character);
            //Add two parties back to our EntityManager
            EntityManager.Instance.AddEntitiesToMap(Map.ID, Map.InstanceID, Blue.Session.Character);
            EntityManager.Instance.AddEntitiesToMap(Map.ID, Map.InstanceID, Red.Session.Character);
        }

        private void SendPlayersBack()
        {
            if(Info.Blue != null)
            {
                Blue.Session.Character.Position = bluePos;
                Blue.Session.Character.Direction = blueDir;

                sourceBlueMap.CharacterEnter(Blue, Blue.Session.Character);
                EntityManager.Instance.AddEntitiesToMap(sourceBlueMap.ID, sourceBlueMap.InstanceID, Blue.Session.Character);
            }

            if(Info.Red != null)
            {
                Red.Session.Character.Position = redPos;
                Red.Session.Character.Direction = redDir;

                sourceRedMap.CharacterEnter(Red, Red.Session.Character);
                EntityManager.Instance.AddEntitiesToMap(sourceRedMap.ID, sourceRedMap.InstanceID, Red.Session.Character);
            }
        }

        internal void Update()
        {
            if(ArenaStat == ArenaStatus.InProgress)
            {
                UpdateRound();
            }
            if(ArenaStat == ArenaStatus.Settling)
            {
                ArenaManager.Instance.RemoveArena(Info.arenaId);
            }
        }

        private void UpdateRound()
        {
            if(RoundStat == ArenaRoundStatus.Ready)
            {
                timer -= Time.deltaTime;
                if(timer <= 0)
                {
                    RoundStat = ArenaRoundStatus.InProgress;
                    timer = ROUND_TIME;
                    Log.InfoFormat("Arena:[{0}] Round[{1}] Start", Info.arenaId, Round);
                    ArenaService.Instance.SendArenaRoundStart(this);
                }
            }
            else if(RoundStat == ArenaRoundStatus.InProgress)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    RoundStat = ArenaRoundStatus.Settling;
                    timer = SETTLING_TIME;
                    Log.InfoFormat("Arena:[{0}] Round[{1}] Over", Info.arenaId, Round);
                    ArenaService.Instance.SendArenaRoundOver(this);
                }
            }
            else if(RoundStat == ArenaRoundStatus.Settling)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    RoundSettled();
                }
            }
        }

        public void RoundSettled()
        {
            if (Round >= 3)
                ArenaSettled();
            else
            {
                PlayerLeaveMap(Blue);
                PlayerLeaveMap(Red);
                SendPlayersInArena();
                ToNextRound();
            }
        }

        public void SetEntityReady(int entityId)
        {
            if (Red.Session.Character.entityId == entityId)
                redReady = true;
            if (Blue.Session.Character.entityId == entityId)
                blueReady = true;

            if(Ready)
            {
                ArenaStat = ArenaStatus.InProgress;
                Round = 0;
                ToNextRound();
            }
        }

        private void ToNextRound()
        {
            Round++;
            timer = READY_COUNT_DOWN;
            RoundStat = ArenaRoundStatus.Ready;
            Log.InfoFormat("Arena:[{0}] Round[{1}] Ready", Info.arenaId, Round);
            ArenaService.Instance.SendArenaReady(this);
        }

        private void ArenaSettled()
        {
            ArenaStat = ArenaStatus.Settling;
            ArenaService.Instance.SendArenaOver(this);
        }

        private void OnOnePartyDown(int eid)
        {
            if(Red.Session.Character.entityId == eid)
            {
                Info.Red.Score++;
            }

            if (Blue.Session.Character.entityId == eid)
            {
                Info.Blue.Score++;
            }

            RoundSettled();
        }

        private void OnOnePartyLeftGame(int eid)
        {
            timer = SETTLING_TIME;
            RoundStat = ArenaRoundStatus.Settling;
            Round = 3;

            if (Red.Session.Character.entityId == eid)
            {
                Info.Red = null;
            }

            if (Blue.Session.Character.entityId == eid)
            {
                Info.Blue = null;
            }
        }
    }
}
