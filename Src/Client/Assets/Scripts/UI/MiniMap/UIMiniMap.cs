using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Managers;

public class UIMiniMap : MonoBehaviour
{
    public BoxCollider miniMapBoundingBox;
    public Image miniMap;
    public Image arrow;
    public Text mapName;

    private Transform playerTrans;
    private void Start()
    {
        InitMap();
    }

    private void InitMap()
    {
        mapName.text = User.Instance.currMap.Name;
        if(miniMap.overrideSprite == null)
        {
            var sprite = MiniMapManager.Instance.LoadCurrentMiniMap();
            miniMap.overrideSprite = sprite;
        }

        miniMap.SetNativeSize();
        miniMap.transform.localPosition = Vector3.zero;
    }

    private void Update()
    {
        if(playerTrans == null) playerTrans = MiniMapManager.Instance.playerTransform;
        if (miniMapBoundingBox == null || playerTrans == null) return;

        float mapWidth = miniMapBoundingBox.bounds.size.x;
        float mapHeight = miniMapBoundingBox.bounds.size.z;

        float relativeX = playerTrans.position.x - miniMapBoundingBox.bounds.min.x;
        float relativeY = playerTrans.position.z - miniMapBoundingBox.bounds.min.z;

        float pivotX = relativeX / mapWidth;
        float pivotY = relativeY / mapHeight;

        miniMap.rectTransform.pivot = new Vector2(pivotX, pivotY);
        miniMap.rectTransform.localPosition = Vector3.zero;
        arrow.transform.eulerAngles = new Vector3(0,0,-playerTrans.eulerAngles.y);
    }
}
