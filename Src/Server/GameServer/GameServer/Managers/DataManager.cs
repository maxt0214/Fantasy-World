using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
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

            json = File.ReadAllText(DataPath + "NPCDefine.txt");
            NPCs = JsonConvert.DeserializeObject<Dictionary<int, NPCDefine>>(json);

            json = File.ReadAllText(DataPath + "ItemDefine.txt");
            Items = JsonConvert.DeserializeObject<Dictionary<int, ItemDefine>>(json);

            json = File.ReadAllText(DataPath + "ShopDefine.txt");
            Shops = JsonConvert.DeserializeObject<Dictionary<int, ShopDefine>>(json);

            json = File.ReadAllText(DataPath + "ShopItemDefine.txt");
            ShopItems = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, ShopItemDefine>>>(json);
        }
    }
}