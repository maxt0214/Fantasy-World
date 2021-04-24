using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

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
}
