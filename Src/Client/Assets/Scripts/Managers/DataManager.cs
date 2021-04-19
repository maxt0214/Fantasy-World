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
    public string DataPath;
    public Dictionary<int, MapDefine> Maps = null;
    public Dictionary<int, CharacterDefine> Characters = null;
    public Dictionary<int, TeleporterDefine> Teleporters = null;
    public Dictionary<int, Dictionary<int, SpawnPointDefine>> SpawnPoints = null;


    public DataManager()
    {
        DataPath = "Data/";
        Debug.LogFormat("DataManager > DataManager()");
    }

    public void Load()
    {
        string json = File.ReadAllText(DataPath + "MapDefine.txt");
        Maps = JsonConvert.DeserializeObject<Dictionary<int, MapDefine>>(json);

        json = File.ReadAllText(DataPath + "CharacterDefine.txt");
        Characters = JsonConvert.DeserializeObject<Dictionary<int, CharacterDefine>>(json);

        //json = File.ReadAllText(DataPath + "TeleporterDefine.txt");
        //Teleporters = JsonConvert.DeserializeObject<Dictionary<int, TeleporterDefine>>(json);

        //json = File.ReadAllText(DataPath + "SpawnPointDefine.txt");
        //SpawnPoints = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SpawnPointDefine>>> (json);
    }


    public IEnumerator LoadData()
    {
        string json = File.ReadAllText(DataPath + "MapDefine.txt");
        Maps = JsonConvert.DeserializeObject<Dictionary<int, MapDefine>>(json);

        yield return null;

        json = File.ReadAllText(DataPath + "CharacterDefine.txt");
        Characters = JsonConvert.DeserializeObject<Dictionary<int, CharacterDefine>>(json);

        yield return null;

        //json = File.ReadAllText(DataPath + "TeleporterDefine.txt");
        //Teleporters = JsonConvert.DeserializeObject<Dictionary<int, TeleporterDefine>>(json);

        //yield return null;

        //json = File.ReadAllText(DataPath + "SpawnPointDefine.txt");
        //SpawnPoints = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SpawnPointDefine>>>(json);

        //yield return null;
    }

#if UNITY_EDITOR
    public void SaveTeleporters()
    {
        string json = JsonConvert.SerializeObject(Teleporters, Formatting.Indented);
        File.WriteAllText(DataPath + "TeleporterDefine.txt", json);
    }

    public void SaveSpawnPoints()
    {
        string json = JsonConvert.SerializeObject(SpawnPoints, Formatting.Indented);
        File.WriteAllText(DataPath + "SpawnPointDefine.txt", json);
    }

#endif
}
