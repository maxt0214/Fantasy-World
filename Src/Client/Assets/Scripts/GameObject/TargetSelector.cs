using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using System;

public class TargetSelector : MonoSingleton<TargetSelector>
{
    private Projector projector;

    private bool active = false;

    private Vector3 center;
    private Vector3 offset = new Vector3(0,2f,0f);
    private float range;
    private float size;

    protected Action<Vector3> selectPoint;

    protected override void OnStart()
    {
        projector = GetComponentInChildren<Projector>();
        projector.gameObject.SetActive(active);
    }

    public void Activate(bool ifActive)
    {
        active = ifActive;
        if (projector == null) return;

        projector.gameObject.SetActive(active);
        projector.orthographicSize = size * 0.5f;
    }

    void Update()
    {
        if(!active || projector == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Terrain")))
        {
            Vector3 hitPoint = hit.point;
            Vector3 dis = hitPoint - center;

            if(dis.magnitude > range)
            {
                hitPoint = center + dis.normalized * range;
            }

            projector.gameObject.transform.position = hitPoint + offset;
            if(Input.GetMouseButtonDown(0))
            {
                selectPoint(hitPoint);
                Activate(false);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Activate(false);
        }
    }

    public static void ShowSelector(Vector3Int center, int r, int s, Action<Vector3> onPositionSelected)
    {
        if (Instance == null) return;

        Instance.center = GameObjectTool.LogicUnitToWorld(center);
        Instance.range = GameObjectTool.LogicUnitToWorld(r);
        Instance.size = GameObjectTool.LogicUnitToWorld(s);
        Instance.selectPoint = onPositionSelected;
        Instance.Activate(true);
    }
}
