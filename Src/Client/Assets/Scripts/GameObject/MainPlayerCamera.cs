using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;

public class MainPlayerCamera : MonoSingleton<MainPlayerCamera>
{
    public Camera playerCam;
    public Transform viewPoint;

    public GameObject player;

    public float followSpd = 5f;
    public float rotateAngle = 5f;

    Quaternion yaw = Quaternion.identity;

    private void LateUpdate()
    {
        if (player == null && User.Instance.currentCharacterObj != null)
        {
            player = User.Instance.currentCharacterObj.gameObject;
        }
        if (player == null) return;

        transform.position = Vector3.Lerp(transform.position, player.transform.position, Time.deltaTime * followSpd);

        if(Input.GetMouseButton(1))
        {
            Vector3 angleBase = transform.localRotation.eulerAngles;
            transform.localRotation = Quaternion.Euler(angleBase.x - Input.GetAxis("Mouse Y") * rotateAngle, angleBase.y - Input.GetAxis("Mouse X") * rotateAngle, 0);
            Vector3 angle = transform.localRotation.eulerAngles - player.transform.localRotation.eulerAngles;
            angle.z = 0;
            yaw = Quaternion.Euler(angle);
        } else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, player.transform.rotation * yaw, Time.deltaTime * followSpd);
        }

        if(Input.GetAxis("Vertical") > 0.01f)
        {
            yaw = Quaternion.Lerp(yaw, Quaternion.identity, Time.deltaTime * followSpd);
        }
    }
}
