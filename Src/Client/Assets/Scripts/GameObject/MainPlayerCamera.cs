using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;

public class MainPlayerCamera : MonoSingleton<MainPlayerCamera>
{
    public Camera playerCam;
    public Transform viewPoint;

    public GameObject player;

    private void LateUpdate()
    {
        if (player == null)
        {
            player = User.Instance.currentCharacterObj;
        }
        if (player == null) return;

        transform.position = player.transform.position;
        transform.rotation = player.transform.rotation;
    }
}
