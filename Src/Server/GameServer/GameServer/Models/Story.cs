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
    class Story
    {
        public Map Map;
        public int StoryId;
        public int InstanceId;
        public NetConnection<NetSession> Owner;
        public StoryDefine Def;

        private Map sourceMap;
        private Vector3Int pos;
        private Vector3Int dir;

        private const int teleId = 12;

        private Quest quest;

        private float timer;
        private bool storyOver = false; 

        public Story(Map map, int storyId, int idx, NetConnection<NetSession> owner)
        {
            Map = map;
            StoryId = storyId;
            InstanceId = idx;
            Owner = owner;
            Def = DataManager.Instance.Stories[StoryId];
            timer = Def.LimitTime;
        }

        public void PlayerIn()
        {
            pos = Owner.Session.Character.Position;
            dir = Owner.Session.Character.Direction;
            sourceMap = PlayerLeaveMap(Owner);

            TeleporterDefine tele = DataManager.Instance.Teleporters[teleId];
            Owner.Session.Character.Position = tele.Position;
            Owner.Session.Character.Direction = tele.Direction;

            Map.AddCharaToMap(Owner, Owner.Session.Character);
            Map.CharacterEnter(Owner, Owner.Session.Character);

            EntityManager.Instance.AddEntitiesToMap(Map.ID, Map.InstanceID, Owner.Session.Character);
            quest = Owner.Session.Character.QuestManager.AcceptQuest(Def.Quest);
            if (quest.Stat == QuestStatus.Finished)
                quest.Init();
        }

        private Map PlayerLeaveMap(NetConnection<NetSession> player)
        {
            var currMap = MapManager.Instance[player.Session.Character.Info.mapId];
            currMap.CharacterLeave(player.Session.Character);
            EntityManager.Instance.RemoveEntityInMap(currMap.ID, currMap.InstanceID, player.Session.Character);
            return currMap;
        }

        public void PlayerOut()
        {
            PlayerLeaveMap(Owner);
            Owner.Session.Character.Position = pos;
            Owner.Session.Character.Direction = dir;

            sourceMap.AddCharaToMap(Owner, Owner.Session.Character);
            sourceMap.CharacterEnter(Owner, Owner.Session.Character);

            EntityManager.Instance.AddEntitiesToMap(sourceMap.ID, sourceMap.InstanceID, Owner.Session.Character);
        }

        internal void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                if(storyOver)
                {
                    PlayerOut();
                    StoryManager.Instance.RemoveStory(StoryId, InstanceId);
                } else
                {
                    Clear();
                }
            }
        }

        public void Clear()
        {
            if (storyOver)
                return;
            quest.Submit();
            QuestService.Instance.SendSubmitQuest(Owner,quest);
            timer = 6f;
            storyOver = true;
        }
    }
}
