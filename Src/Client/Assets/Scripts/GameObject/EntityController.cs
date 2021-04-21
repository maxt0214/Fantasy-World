using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using SkillBridge.Message;
using Managers;

public class EntityController : MonoBehaviour , IEntityNotify
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

    public void OnEntityEvent(EntityEvent entityEvent)
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
        }
    }

    public void OnEntityRemoved()
    {
        if (UIWorldElementManager.Instance != null)
            UIWorldElementManager.Instance.RemoveCharacterNameBar(transform);
        Destroy(gameObject);
    }
}
