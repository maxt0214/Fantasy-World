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
            miniMap.overrideSprite = MiniMapManager.Instance.LoadCurrentMiniMap();
        }

        miniMap.SetNativeSize();
        miniMap.transform.localPosition = Vector3.zero;
        playerTrans = User.Instance.currentCharacterObj.transform;
    }

    private void Update()
    {
        float mapWidth = miniMapBoundingBox.size.x;
        float mapHeight = miniMapBoundingBox.size.y;

        float relativeX = playerTrans.position.x - miniMapBoundingBox.bounds.min.x;
        float relativeY = playerTrans.position.z - miniMapBoundingBox.bounds.min.z;

        float pivotX = relativeX / mapWidth;
        float pivotY = relativeY / mapHeight;

        miniMap.rectTransform.pivot = new Vector2(pivotX, pivotY);
        miniMap.rectTransform.localPosition = Vector3.zero;
    }
}
