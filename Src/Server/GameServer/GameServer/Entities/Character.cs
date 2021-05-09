using Common.Data;
using GameServer.Core;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entities
{
    class Character : CharacterBase
    {
        public TCharacter Data;
        public ItemManager itemManager;

        public StatusManager StatusManager;
        public QuestManager QuestManager;

        public Character(CharacterType type,TCharacter cha):
            base(new Vector3Int(cha.MapPosX, cha.MapPosY, cha.MapPosZ),new Vector3Int(100,0,0))
        {
            Data = cha;
            Info = new NCharacterInfo();
            Info.Type = type;
            Info.Id = cha.ID;
            Info.Name = cha.Name;
            Info.Level = 10;//cha.Level;
            Info.Tid = cha.TID;
            Info.Class = (CharacterClass)cha.Class;
            Info.mapId = cha.MapID;
            Info.Gold = cha.Gold;
            Info.Entity = EntityData;
            Define = DataManager.Instance.Characters[Info.Tid];

            itemManager = new ItemManager(this);
            itemManager.GetItemInfos(Info.Items);

            Info.Bag = new NBagInfo();
            Info.Bag.Unlocked = cha.Bag.Unlocked;
            Info.Bag.Items = cha.Bag.items;
            Info.Equips = Data.Equips;
            QuestManager = new QuestManager(this);
            QuestManager.GetQuestInfos(Info.Quests);
            StatusManager = new StatusManager(this);
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
    }
}
