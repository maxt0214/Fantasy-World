using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

class NavPathRenderer : MonoSingleton<NavPathRenderer>
{
    private LineRenderer pathRenderer;
    private NavMeshPath path;

    protected override void OnStart()
    {
        pathRenderer = GetComponent<LineRenderer>();
        pathRenderer.enabled = false;
    }

    public void SetPath(NavMeshPath pendingPath, Vector3 target)
    {
        path = pendingPath;
        if(path == null)
        {
            pathRenderer.enabled = false;
            pathRenderer.positionCount = 0;
        } else
        {
            pathRenderer.enabled = true;
            pathRenderer.positionCount = path.corners.Length + 1;
            pathRenderer.SetPositions(path.corners);
            pathRenderer.SetPosition(path.corners.Length, target);
            for(int i = 0; i < pathRenderer.positionCount; i++)
            {
                pathRenderer.SetPosition(i, pathRenderer.GetPosition(i) + Vector3.up * 0.2f);
            }
        }
    }
}
