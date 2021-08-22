using System.Collections;
using System.Collections.Generic;
using System.IO;
using Common;
using Common.Data;

using Newtonsoft.Json;
namespace GameServer.Managers
{
    public class DataManager : Singleton<DataManager>
    {
        internal string DataPath;
        internal Dictionary<int, MapDefine> Maps = null;
        internal Dictionary<int, CharacterDefine> Characters = null;
        internal Dictionary<int, TeleporterDefine> Teleporters = null;
        public Dictionary<int, Dictionary<int, SpawnPointDefine>> SpawnPoints = null;
        public Dictionary<int, Dictionary<int,SpawnRuleDefine>> SpawnRules = null;
        public Dictionary<int, NPCDefine> NPCs = null;
        public Dictionary<int, ItemDefine> Items = null;
        public Dictionary<int, ShopDefine> Shops = null;
        public Dictionary<int, Dictionary<int, ShopItemDefine>> ShopItems = null;
        public Dictionary<int, EquipDefine> Equips = null;
        public Dictionary<int, QuestDefine> Quests = null;
        public Dictionary<int, RideDefine> Rides = null;
        public Dictionary<int, Dictionary<int, SkillDefine>> Skills = null;
        public Dictionary<int, BuffDefine> Buffs = null;

        public DataManager()
        {
            DataPath = "Data/";
            Log.Info("DataManager > DataManager()");
        }

        internal void Load()
        {
            string json = File.ReadAllText(DataPath + "MapDefine.txt");
            Maps = JsonConvert.DeserializeObject<Dictionary<int, MapDefine>>(json);

            json = File.ReadAllText(DataPath + "CharacterDefine.txt");
            Characters = JsonConvert.DeserializeObject<Dictionary<int, CharacterDefine>>(json);

            json = File.ReadAllText(DataPath + "TeleporterDefine.txt");
            Teleporters = JsonConvert.DeserializeObject<Dictionary<int, TeleporterDefine>>(json);

            json = File.ReadAllText(DataPath + "SpawnPointDefine.txt");
            SpawnPoints = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SpawnPointDefine>>>(json);

            json = File.ReadAllText(DataPath + "SpawnRuleDefine.txt");
            SpawnRules = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SpawnRuleDefine>>>(json);

            json = File.ReadAllText(DataPath + "NPCDefine.txt");
            NPCs = JsonConvert.DeserializeObject<Dictionary<int, NPCDefine>>(json);

            json = File.ReadAllText(DataPath + "ItemDefine.txt");
            Items = JsonConvert.DeserializeObject<Dictionary<int, ItemDefine>>(json);

            json = File.ReadAllText(DataPath + "ShopDefine.txt");
            Shops = JsonConvert.DeserializeObject<Dictionary<int, ShopDefine>>(json);

            json = File.ReadAllText(DataPath + "ShopItemDefine.txt");
            ShopItems = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, ShopItemDefine>>>(json);

            json = File.ReadAllText(DataPath + "EquipDefine.txt");
            Equips = JsonConvert.DeserializeObject<Dictionary<int, EquipDefine>>(json);

            json = File.ReadAllText(DataPath + "QuestDefine.txt");
            Quests = JsonConvert.DeserializeObject<Dictionary<int, QuestDefine>>(json);

            json = File.ReadAllText(DataPath + "RideDefine.txt");
            Rides = JsonConvert.DeserializeObject<Dictionary<int, RideDefine>>(json);

            json = File.ReadAllText(DataPath + "SkillDefine.txt");
            Skills = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SkillDefine>>>(json);

            json = File.ReadAllText(DataPath + "BuffDefine.txt");
            Buffs = JsonConvert.DeserializeObject<Dictionary<int, BuffDefine>>(json);
        }
    }
}