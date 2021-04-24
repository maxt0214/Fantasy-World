using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;

public class MapController : MonoBehaviour
{
    public Collider mapBoundingBox;

    void Start()
    {
        MiniMapManager.Instance.UpdateMiniMap(mapBoundingBox);
    }

    void Update()
    {
        
    }
}
