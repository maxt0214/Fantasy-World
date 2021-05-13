using Common.Data;
using GameServer.Core;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using Network;
using GameServer.Models;
using Common;

namespace GameServer.Entities
{
    class Character : CharacterBase, IPostResponser
    {
        public TCharacter Data;
        public ItemManager itemManager;

        public StatusManager StatusManager;
        public QuestManager QuestManager;
        public FriendManager FriendManager;

        public Team team;
        public int teamUpdateTS;

        public Character(CharacterType type,TCharacter cha):
            base(new Vector3Int(cha.MapPosX, cha.MapPosY, cha.MapPosZ),new Vector3Int(100,0,0))
        {
            Data = cha;
            Id = cha.ID;
            Info = new NCharacterInfo();
            Info.Type = type;
            Info.Id = cha.ID;
            Info.EntityId = entityId;
            Info.Name = cha.Name;
            Info.Level = 10;//cha.Level;
            Info.ConfigId = cha.TID;
            Info.Class = (CharacterClass)cha.Class;
            Info.mapId = cha.MapID;
            Info.Gold = cha.Gold;
            Info.Entity = EntityData;
            Define = DataManager.Instance.Characters[Info.ConfigId];

            itemManager = new ItemManager(this);
            itemManager.GetItemInfos(Info.Items);

            Info.Bag = new NBagInfo();
            Info.Bag.Unlocked = cha.Bag.Unlocked;
            Info.Bag.Items = cha.Bag.items;
            Info.Equips = Data.Equips;
            QuestManager = new QuestManager(this);
            QuestManager.GetQuestInfos(Info.Quests);
            StatusManager = new StatusManager(this);
            FriendManager = new FriendManager(this);
            FriendManager.GetFriendInfos(Info.Friends);
        }

        public long Gold
        {
            get
            {
                return Data.Gold;
            }
            set
            {
                if (Data.Gold == value)
                    return;
                StatusManager.ChangeGoldStatus((int)(value - Data.Gold));
                Data.Gold = value;
            }
        }

        public NCharacterInfo GetBasicInfo()
        {
            return new NCharacterInfo()
            {
                Id = Id,
                Name = Info.Name,
                Class = Info.Class,
                Level = Info.Level,
            };
        }

        public void PostProcess(NetMessageResponse response)
        {
            Log.InfoFormat("PostProccess > Character[{0}]:{1}", Id, Info.Name);
            FriendManager.PostProcess(response);

            if(team != null)
            {
                Log.InfoFormat("PostProccess > Character[{0}]:{1} LastUpdate:{2} vs TeamUpdate:{3}", Id, Info.Name, teamUpdateTS, team.timeStamp);
                if(teamUpdateTS < team.timeStamp)
                {
                    teamUpdateTS = team.timeStamp;
                    team.PostProcess(response);
                }
            }

            if (StatusManager.HasStatus)
                StatusManager.PostProcess(response);
        }

        /// <summary>
        /// Will Be Called When The Character Left
        /// </summary>
        public void Clear()
        {
            FriendManager.OfflineNotify();
            if(team != null) team.MemberLeft(this);
        }
    }
}
