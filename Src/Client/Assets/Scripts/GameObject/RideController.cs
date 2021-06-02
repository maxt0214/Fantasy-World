using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using SkillBridge.Message;
using Services;
using Managers;
using System;

public class RideController : MonoBehaviour
{
    public Transform mountPoint;
    public EntityController rider;
    public Vector3 offset;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (mountPoint == null || rider == null) return;

        rider.SetRidePosition(mountPoint.position + mountPoint.TransformDirection(offset));
    }

    public void SetRider(EntityController rider)
    {
        this.rider = rider;
    }

    public void OnEntityEvent(EntityEvent entityEvent, int param)
    {
        switch (entityEvent)
        {
            case EntityEvent.Idle:
                anim.SetBool("Move", false);
                anim.SetTrigger("Idle");
                break;
            case EntityEvent.MoveFwd:
                anim.SetBool("Move", true);
                break;
            case EntityEvent.MoveBack:
                anim.SetBool("Move", true);
                break;
            case EntityEvent.Jump:
                anim.SetTrigger("Jump");
                break;
        }
    }
}
