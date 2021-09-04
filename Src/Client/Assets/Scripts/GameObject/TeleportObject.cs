using Common.Data;
using Managers;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TeleportObject : MonoBehaviour
{
    public int ID;
    private Mesh mesh = null;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
    }
    
    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        var playerController = other.GetComponent<PlayerInputController>();
        if(playerController != null && playerController.isActiveAndEnabled)
        {
            TeleporterDefine td = DataManager.Instance.Teleporters[ID];
            if(td == null)
            {
                Debug.LogErrorFormat("TeleportObject: Character:{0} tries to enter Teleporter:{1} when its TeleporterDefine does not exist!", playerController.characterEntity.Info.Name, ID);
                return;
            } else
            {
                if (DataManager.Instance.Teleporters.ContainsKey(td.LinkTo))
                {
                    if (td.MapID != 4)
                        MapService.Instance.TeleportFrom(td.ID);
                    else
                        StoryManager.Instance.FinishStory();
                }
                else
                    Debug.LogErrorFormat("Teleporter:{0} is linked to Teleporter:{1} which does not exist!", ID, td.LinkTo);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if(mesh != null)
        {
            Gizmos.DrawMesh(mesh, transform.position + new Vector3(0, transform.localScale.y / 2, 0), transform.rotation, transform.localScale);
        }
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.ArrowHandleCap(0, transform.position, transform.rotation, 5, EventType.Repaint);
    }
#endif
}
