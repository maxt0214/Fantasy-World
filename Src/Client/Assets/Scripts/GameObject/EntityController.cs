using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using SkillBridge.Message;
using Managers;
using System;

public class EntityController : MonoBehaviour , IEntityNotify , IEntityController
{
    public Animator anim;
    private AnimationState currentBaseState;
    public Rigidbody rb;

    public Entity entity;

    public Vector3 position;
    public Vector3 direction;
    private Quaternion rotation;

    public Vector3 lastPosition;
    private Quaternion lastRotation;

    public float speed;
    public float animSpeed = 1.5f;
    public float jumpForce = 3f;

    public bool isLocalPlayer = false;

    public RideController rideController;
    public int currRide = 0;
    public Transform rideBone;

    public EntityEffectManager EffectMgr;

    private void Start()
    {
        if (entity != null)
        {
            EntityManager.Instance.RegisterEntityChangeNotify(entity.entityId,this);
            UpdateTransform();
        }
            

        if(!isLocalPlayer) //We do not want gravity to take effect when the entity is not our player
            rb.useGravity = false;
    }
    
    private void UpdateTransform()
    {
        position = GameObjectTool.LogicUnitToWorld(entity.position);
        direction = GameObjectTool.LogicUnitToWorld(entity.direction);

        rb.MovePosition(position);
        transform.forward = direction;
        lastPosition = position;
        lastRotation = rotation;
    }

    private void OnDestroy()
    {
        if (entity != null)
            Debug.LogFormat("{0} OnDestroy: ID: {1} Position: {2} Direction: {3} Speed: {4}", name, entity.entityId, entity.position, entity.direction, entity.speed);

        if (UIWorldElementManager.Instance != null)
            UIWorldElementManager.Instance.RemoveCharacterNameBar(transform);
    }

    private void FixedUpdate()
    {
        if (entity == null)
            return;

        entity.OnUpdate(Time.fixedDeltaTime);

        if (!isLocalPlayer)
            UpdateTransform();
    }

    public void OnEntityEvent(EntityEvent entityEvent, int param)
    {
        switch(entityEvent)
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
            case EntityEvent.Ride:
                Ride(param);
                break;
        }
        if (rideController != null) rideController.OnEntityEvent(entityEvent, param);
    }

    public void OnEntityRemoved()
    {
        if (UIWorldElementManager.Instance != null)
            UIWorldElementManager.Instance.RemoveCharacterNameBar(transform);
        Destroy(gameObject);
    }

    public void OnEntityChange(Entity entity)
    {
        Debug.LogFormat("OnEntityChange: EntityID:{0} Pos:{1} Dir:{2}", entity.entityId, entity.EntityData.Position, entity.EntityData.Direction);
    }

    public void Ride(int rideId)
    {
        if (currRide == rideId) return;
        currRide = rideId;
        if(rideId > 0)
        {
            rideController = GameObjectManager.Instance.LoadRide(rideId,transform);
        } else
        {
            Destroy(rideController.gameObject);
            rideController = null;
        }

        if(rideController == null)
        {
            anim.transform.localPosition = Vector3.zero;
            anim.SetLayerWeight(1,0);
        } else
        {
            rideController.SetRider(this);
            anim.SetLayerWeight(1,1);
        }
    }

    private void OnMouseDown()
    {
        Creature target = entity as Creature;
        if (target.IsLocalPlayer) return;
        BattleManager.Instance.Target = target;
    }

    public void SetRidePosition(Vector3 pos)
    {
        anim.transform.position = pos + (anim.transform.position - rideBone.position);
    }

    public void PlayAnim(string name)
    {
        anim.SetTrigger(name);
    }

    public void SetStandby(bool ifStandby)
    {
        anim.SetBool("Standby",ifStandby);
    }

    public void UpdateDirection()
    {
        direction = GameObjectTool.LogicUnitToWorld(entity.direction);
        transform.forward = direction;
    }

    public void PlayEffect(EffectType type, string name, Creature target, float duration)
    {
        Transform trans = target.Controller.GetTransform();
        EffectMgr.PlayEffect(type, name, trans, duration);
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
