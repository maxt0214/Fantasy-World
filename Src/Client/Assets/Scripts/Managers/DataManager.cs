using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Text;
using System;
using System.IO;

using Common.Data;

using Newtonsoft.Json;

public class DataManager : Singleton<DataManager>
{
    public string DataPath;//client
    public string CommonPath;//common data
    public string ServerPath;//server
    public Dictionary<int, MapDefine> Maps = null;
    public Dictionary<int, CharacterDefine> Characters = null;
    public Dictionary<int, TeleporterDefine> Teleporters = null;
    public Dictionary<int, Dictionary<int, SpawnPointDefine>> SpawnPoints = null;
    public Dictionary<int, Dictionary<int, SpawnRuleDefine>> SpawnRules = null;
    public Dictionary<int, NPCDefine> NPCs = null;
    public Dictionary<int, ItemDefine> Items = null;
    public Dictionary<int, ShopDefine> Shops = null;
    public Dictionary<int, Dictionary<int, ShopItemDefine>> ShopItems = null;
    public Dictionary<int, EquipDefine> Equips = null;
    public Dictionary<int, QuestDefine> Quests = null;
    public Dictionary<int, RideDefine> Rides = null;

    public DataManager()
    {
        DataPath = "Data/";
        CommonPath = "../Data/Data/";
        ServerPath = "../Server/GameServer/GameServer/bin/Debug/Data/";
        Debug.LogFormat("DataManager > DataManager()");
    }

    public void Load()
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
    }


    public IEnumerator LoadData()
    {
        string json = File.ReadAllText(DataPath + "MapDefine.txt");
        Maps = JsonConvert.DeserializeObject<Dictionary<int, MapDefine>>(json);

        yield return null;

        json = File.ReadAllText(DataPath + "CharacterDefine.txt");
        Characters = JsonConvert.DeserializeObject<Dictionary<int, CharacterDefine>>(json);

        yield return null;

        json = File.ReadAllText(DataPath + "TeleporterDefine.txt");
        Teleporters = JsonConvert.DeserializeObject<Dictionary<int, TeleporterDefine>>(json);

        yield return null;

        json = File.ReadAllText(DataPath + "SpawnPointDefine.txt");
        SpawnPoints = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SpawnPointDefine>>>(json);

        yield return null;

        json = File.ReadAllText(DataPath + "SpawnRuleDefine.txt");
        SpawnRules = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SpawnRuleDefine>>>(json);

        yield return null;

        json = File.ReadAllText(DataPath + "NPCDefine.txt");
        NPCs = JsonConvert.DeserializeObject<Dictionary<int, NPCDefine>>(json);

        yield return null;

        json = File.ReadAllText(DataPath + "ItemDefine.txt");
        Items = JsonConvert.DeserializeObject<Dictionary<int, ItemDefine>>(json);

        yield return null;

        json = File.ReadAllText(DataPath + "ShopDefine.txt");
        Shops = JsonConvert.DeserializeObject<Dictionary<int, ShopDefine>>(json);

        yield return null;

        json = File.ReadAllText(DataPath + "ShopItemDefine.txt");
        ShopItems = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, ShopItemDefine>>>(json);

        yield return null;

        json = File.ReadAllText(DataPath + "EquipDefine.txt");
        Equips = JsonConvert.DeserializeObject<Dictionary<int, EquipDefine>>(json);

        yield return null;

        json = File.ReadAllText(DataPath + "QuestDefine.txt");
        Quests = JsonConvert.DeserializeObject<Dictionary<int, QuestDefine>>(json);

        yield return null;

        json = File.ReadAllText(DataPath + "RideDefine.txt");
        Rides = JsonConvert.DeserializeObject<Dictionary<int, RideDefine>>(json);

        yield return null;
    }

#if UNITY_EDITOR
    public void SaveTeleporters()
    {
        string json = JsonConvert.SerializeObject(Teleporters, Formatting.Indented);
        File.WriteAllText(DataPath + "TeleporterDefine.txt", json);
        //Copy to server
        File.WriteAllText(ServerPath + "TeleporterDefine.txt", json);
    }

    public void SaveSpawnPoints()
    {
        string json = JsonConvert.SerializeObject(SpawnPoints, Formatting.Indented);
        File.WriteAllText(DataPath + "SpawnPointDefine.txt", json);
        //Copy to server
        File.WriteAllText(ServerPath + "TeleporterDefine.txt", json);
    }

    public void SaveReachableAreas(Dictionary<int, List<Vector3Int>> area)
    {
        string json = JsonConvert.SerializeObject(area, Formatting.Indented);
        File.WriteAllText(ServerPath + "Walkable.txt", json);
    }
#endif
}
