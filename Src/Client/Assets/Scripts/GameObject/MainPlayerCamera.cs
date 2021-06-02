using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;

public class MainPlayerCamera : MonoSingleton<MainPlayerCamera>
{
    public Camera playerCam;
    public Transform viewPoint;

    public GameObject player;

    public float rotateAngle = 1f;

    private void LateUpdate()
    {
        if (player == null && User.Instance.currentCharacterObj != null)
        {
            player = User.Instance.currentCharacterObj.gameObject;
        }
        if (player == null) return;

        transform.position = player.transform.position;
        transform.rotation = player.transform.rotation;

        //float xRot = Input.GetAxis("Mouse X");
        //float yRot = Input.GetAxis("Mouse Y");
        //xRot = Mathf.Clamp(xRot, -30, 20);
        //transform.Rotate(yRot * rotateAngle, xRot * rotateAngle, 0);
    }
}
