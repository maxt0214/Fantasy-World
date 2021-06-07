using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Common.Data;
using UnityEngine.AI;

public class MapTools
{
    [MenuItem("Map Tools/Export Teleporters")]
    public static void ExportTeleporters()
    {
        DataManager.Instance.Load();

        Scene currScene = EditorSceneManager.GetActiveScene();
        string currSceneName = currScene.name;
        if(currScene.isDirty)
        {
            EditorUtility.DisplayDialog("Hint","Please Save Scene","Confirm");
            return;
        }

        foreach(var map in DataManager.Instance.Maps)
        {
            string sceneFile = "Assets/Levels/" + map.Value.Resource + ".unity";
            if(!System.IO.File.Exists(sceneFile))
            {
                Debug.LogWarningFormat("Scene:{0} does not exist!", sceneFile);
                continue;
            }

            EditorSceneManager.OpenScene(sceneFile,OpenSceneMode.Single);
            TeleportObject[] teleObjs = Object.FindObjectsOfType<TeleportObject>();
            foreach(var teleObj in teleObjs)
            {
                if(!DataManager.Instance.Teleporters.ContainsKey(teleObj.ID))
                {
                    EditorUtility.DisplayDialog("Error", string.Format("Map:{0} has Teleporter:[{1}] which does not exist!", map.Value.Resource, teleObj.ID), "Confirm");
                    return;
                }

                var teleDefine = DataManager.Instance.Teleporters[teleObj.ID];
                if(teleDefine.MapID != map.Value.ID)
                {
                    EditorUtility.DisplayDialog("Error", string.Format("Map:{0} with Teleporter:[{1}] conflicts with Teleporter:[{2}] in TeleporterDefine!", map.Value.Resource, teleObj.ID, teleDefine.ID), "Confirm");
                    return;
                }
                teleDefine.Position = GameObjectTool.WorldUnitToLogicN(teleObj.transform.position);
                teleDefine.Direction = GameObjectTool.WorldUnitToLogicN(teleObj.transform.forward);
            }
        }
        DataManager.Instance.SaveTeleporters();
        EditorSceneManager.OpenScene("Assets/Levels/" + currSceneName + ".unity");
        EditorUtility.DisplayDialog("Hint", "Teleporter Exportation Over", "Confirm");
    }

    [MenuItem("Map Tools/Export SpawnPoints")]
    public static void ExportSpawnPoints()
    {
        DataManager.Instance.Load();

        Scene currScene = EditorSceneManager.GetActiveScene();
        string currSceneName = currScene.name;
        if (currScene.isDirty)
        {
            EditorUtility.DisplayDialog("Hint", "Please Save Scene", "Confirm");
            return;
        }

        if (DataManager.Instance.SpawnPoints == null)
            DataManager.Instance.SpawnPoints = new Dictionary<int, Dictionary<int, SpawnPointDefine>>();

        foreach (var map in DataManager.Instance.Maps)
        {
            string sceneFile = "Assets/Levels/" + map.Value.Resource + ".unity";
            if (!System.IO.File.Exists(sceneFile))
            {
                Debug.LogWarningFormat("Scene:{0} does not exist!", sceneFile);
                continue;
            }

            EditorSceneManager.OpenScene(sceneFile, OpenSceneMode.Single);
            SpawnPoint[] spawnPoints = Object.FindObjectsOfType<SpawnPoint>();

            if (!DataManager.Instance.SpawnPoints.ContainsKey(map.Value.ID))
            {
                DataManager.Instance.SpawnPoints[map.Value.ID] = new Dictionary<int, SpawnPointDefine>();
            }
            foreach(var sp in spawnPoints)
            {
                if(!DataManager.Instance.SpawnPoints[map.Value.ID].ContainsKey(sp.ID))
                {
                    DataManager.Instance.SpawnPoints[map.Value.ID][sp.ID] = new SpawnPointDefine();
                }

                var def = DataManager.Instance.SpawnPoints[map.Value.ID][sp.ID];
                def.ID = sp.ID;
                def.MapID = map.Value.ID;
                def.Position = GameObjectTool.WorldUnitToLogicN(sp.transform.position);
                def.Direction = GameObjectTool.WorldUnitToLogicN(sp.transform.forward);
            }
        }
        DataManager.Instance.SaveSpawnPoints();
        EditorSceneManager.OpenScene("Assets/Levels/" + currSceneName + ".unity");
        EditorUtility.DisplayDialog("Hint", "Spawn Point Exportation Over", "Confirm");
    }

    static private GameObject root;
    static public Dictionary<int, List<Vector3Int>> navData = new Dictionary<int, List<Vector3Int>>();
    static public Dictionary<int, List<GameObject>> navBoxes = new Dictionary<int, List<GameObject>>();
    [MenuItem("Map Tools/Generate Nav Data")]
    public static void GenerateNavData()
    {
        DataManager.Instance.Load();
        Scene currScene = EditorSceneManager.GetActiveScene();
        string currSceneName = currScene.name;
        if (currScene.isDirty)
        {
            EditorUtility.DisplayDialog("Hint", "Please Save Scene", "Confirm");
            return;
        }

        ClearNavData();

        foreach (var map in DataManager.Instance.Maps)
        {
            string sceneFile = "Assets/Levels/" + map.Value.Resource + ".unity";
            if (!System.IO.File.Exists(sceneFile))
            {
                Debug.LogWarningFormat("Scene:{0} does not exist!", sceneFile);
                continue;
            }

            EditorSceneManager.OpenScene(sceneFile, OpenSceneMode.Single);

            GenerateNavOnMap(map.Value.ID);
            if(!EditorSceneManager.SaveOpenScenes())
            {
                EditorUtility.DisplayDialog("Hint", "Saving Scene Aborted. NavData Generation Aborted", "Confirm");
                ClearNavData();
                return;
            }
        }
        EditorSceneManager.OpenScene("Assets/Levels/" + currSceneName + ".unity");
        EditorUtility.DisplayDialog("Hint", "Generate NavData Over", "Confirm");
    }

    private static void GenerateNavOnMap(int mapId)
    {
        Material red = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended"));
        red.color = Color.red;
        red.SetColor("_TintColor", Color.red);
        red.enableInstancing = true;
        var go = GameObject.Find("MiniMapBoundingBox");
        List<Vector3Int> reachable = new List<Vector3Int>();
        List<GameObject> boxes = new List<GameObject>();
        if (go != null)
        {
            root = new GameObject("Root");
            var bound = go.GetComponent<BoxCollider>();
            float gap = 1f;
            for (float x = bound.bounds.min.x; x < bound.bounds.max.x; x += gap)
            {
                for (float z = bound.bounds.min.z; z < bound.bounds.max.z; z += gap)
                {
                    for (float y = bound.bounds.min.y; y < bound.bounds.max.y + 10; y += gap)
                    {
                        var pos = new Vector3(x, y, z);
                        NavMeshHit hit;
                        if (NavMesh.SamplePosition(pos, out hit, 0.5f, NavMesh.AllAreas))
                        {
                            if (hit.hit)
                            {
                                var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                box.name = "Hit" + hit.mask;
                                box.GetComponent<MeshRenderer>().sharedMaterial = red;
                                box.transform.SetParent(root.transform, true);
                                box.transform.position = pos;
                                box.transform.localScale = Vector3.one * 0.9f;
                                reachable.Add(GameObjectTool.WorldUnitToLogicInt(pos));
                                boxes.Add(box);
                            }
                        }
                    }
                }
            }
        }
        navData[mapId] = reachable;
        navBoxes[mapId] = boxes;
    }

    private static void ClearNavData()
    {
        if (navData.Count > 0 || navBoxes.Count > 0 || root != null)
        {
            navData.Clear();
            navBoxes.Clear();
            GameObject.DestroyImmediate(root);
        }
    }

    [MenuItem("Map Tools/Save Nav Data")]
    public static void SaveNavData()
    {
        if(navData.Count == 0)
        {
            EditorUtility.DisplayDialog("Hint", "Please Click \'Generate Nav Data\' First", "Confirm");
            return;
        }
        DataManager.Instance.SaveReachableAreas(navData);
        navBoxes.Clear();
        if (root != null) GameObject.DestroyImmediate(root);
        EditorUtility.DisplayDialog("Hint", "NavData Saved", "Confirm");
    }
}
