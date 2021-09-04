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
    class Character : Creature, IPostResponser
    {
        public TCharacter Data;

        public ItemManager itemManager;
        public StatusManager StatusManager;
        public QuestManager QuestManager;
        public FriendManager FriendManager;

        public Team team;
        public double teamUpdateTS;

        public Guild guild;
        public double guildUpdateTS;

        public Chat chat;

        public int ride
        {
            get
            {
                return Info.Ride;
            }
            set
            {
                if (Info.Ride == value) return;
                Info.Ride = value;
            }
        }

        public Character(CharacterType type,TCharacter cha):
            base(type, cha.TID, cha.Level, new Vector3Int(cha.MapPosX, cha.MapPosY, cha.MapPosZ),new Vector3Int(100,0,0))
        {
            Data = cha;
            Id = cha.ID;
            Info.Type = type;
            Info.Id = cha.ID;
            Info.EntityId = entityId;
            Info.Name = cha.Name;
            Info.Level = 10;//cha.Level;
            Info.Exp = cha.Exp;
            Info.ConfigId = cha.TID;
            Info.Class = (CharacterClass)cha.Class;
            Info.mapId = cha.MapID;
            Info.Gold = cha.Gold;
            Info.Ride = 0;
            Info.Entity = EntityData;
            Define = DataManager.Instance.Characters[Info.ConfigId];

            itemManager = new ItemManager(this);
            itemManager.GetItemInfos(Info.Items);

            Info.Bag = new NBagInfo();
            Info.Bag.Unlocked = cha.Bag.Unlocked;
            Info.Bag.Items = cha.Bag.items;
            Info.Equips = Data.Equips;
            QuestManager = new QuestManager(this);
            QuestManager.InitQuests(Info.Quests);
            StatusManager = new StatusManager(this);
            FriendManager = new FriendManager(this);
            FriendManager.GetFriendInfos(Info.Friends);

            guild = GuildManager.Instance.GetGuild(Data.GuildId);

            chat = new Chat(this);

            Info.dynamicAttri.Hp = cha.HP;
            Info.dynamicAttri.Mp = cha.MP;
        }

        public void AddExp(int exp)
        {
            Exp += exp;
            CheckLevelUp();
        }

        private void CheckLevelUp()
        {
            long expNeeded = (long)Math.Pow(Level, 3) * 10 + Level * 40 + 50;
            if(Exp > expNeeded)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            Level += 1;
            Log.InfoFormat("Character[{0}]:{1} Leveled Up To: {2}", Info.Id, Info.Name, Level);
            CheckLevelUp();
        }

        public long Exp
        {
            get { return Data.Exp; }
            set
            {
                if (Exp == value) return;
                StatusManager.ChangeExpStatus((int)(value - Data.Exp));
                Data.Exp = value;
                Info.Exp = value;
            }
        }

        public int Level
        {
            get { return Data.Level; }
            private set
            {
                if (Level == value) return;
                StatusManager.ChangeLevelStatus(value - Data.Level);
                Data.Level = value;
                Info.Level = value;
            }
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
                Info.Gold = value;
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

            if (guild != null)
            {
                Log.InfoFormat("PostProccess > Character[{0}]:{1} LastUpdate:{2} vs GuildUpdate:{3}", Id, Info.Name, guildUpdateTS, guild.timeStamp);
                if (Info.Guild == null)
                {
                    Info.Guild = guild.GuildInfo(this);
                    if (response.mapCharacterEnter != null)
                        guildUpdateTS = guild.timeStamp;
                }
                if(guildUpdateTS < guild.timeStamp && response.mapCharacterEnter == null)
                {
                    guildUpdateTS = guild.timeStamp;
                    guild.PostProcess(this,response);
                }
            }

            if (StatusManager.HasStatus)
                StatusManager.PostProcess(response);

            chat.PostProcess(response);
        }

        public delegate void CharacterLeftHandler(int eid);
        public CharacterLeftHandler OnLeft;

        /// <summary>
        /// Will Be Called When The Character Left
        /// </summary>
        public void Clear()
        {
            FriendManager.OfflineNotify();
            if (team != null) team.MemberLeft(this);
            if(guild != null) guild.MemberOffline();
        }
    }
}
