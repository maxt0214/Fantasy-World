using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpawnPoint : MonoBehaviour
{
    public int ID;
    private Mesh mesh = null;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var pos = transform.position + Vector3.up * transform.localScale.y * 0.5f;
        Gizmos.color = Color.red;
        if (mesh != null)
        {
            Gizmos.DrawWireMesh(mesh, pos, transform.rotation, transform.localScale);
        }
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.ArrowHandleCap(0, transform.position, transform.rotation, 1, EventType.Repaint);
        UnityEditor.Handles.Label(pos,"Spawn Point: " + ID);
    }
#endif
}
