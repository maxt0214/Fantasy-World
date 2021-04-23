using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using SkillBridge.Message;
using Services;

[RequireComponent(typeof(EntityController))]
public class PlayerInputController : MonoBehaviour
{
    public Rigidbody rb;
    public Character characterEntity;
    public EntityController entityController { get; set; }
    public CharacterState motionState { get; private set; }

    [Header("Character Motion")]
    public float rotateUnitAngle = 2f;
    public float turnAngle = 10f;
    public bool InAir = false;
    public int speed { get; private set; }

    private void Start()
    {
        motionState = CharacterState.Idle;

        //For local test. character entity should be initialized before the character enters the game
        if (characterEntity == null)
        {
            var charaInfo = new NCharacterInfo();
            charaInfo.Id = 1;
            charaInfo.Name = "Test Character";
            charaInfo.Tid = 1;
            charaInfo.Entity = new NEntity();
            charaInfo.Entity.Position = new NVector3();
            charaInfo.Entity.Direction = new NVector3();
            charaInfo.Entity.Direction.X = 0;
            charaInfo.Entity.Direction.Y = 100;
            charaInfo.Entity.Direction.Z = 0;
            characterEntity = new Character(charaInfo);

            if (entityController != null) entityController.entity = characterEntity;
        }
    }

    private void FixedUpdate()
    {
        if (characterEntity == null)
            return;

        float v = Input.GetAxis("Vertical");
        if(v > 0.01)
        {
            //If not yet moving, then start
            if(motionState != CharacterState.Move)
            {
                //This is to update local movement to entity and to be sent later to the remote.
                motionState = CharacterState.Move;
                characterEntity.MoveForward();
                SendEntityEvent(EntityEvent.MoveFwd);
            }
            //This is to move the player locally.
            rb.velocity = rb.velocity.y * Vector3.up + GameObjectTool.LogicUnitToWorld(characterEntity.direction) * (characterEntity.speed + 9.81f) / 100f;
        } 
        else if(v < -0.01)
        {
            //If not yet moving, then start
            if (motionState != CharacterState.Move)
            {
                motionState = CharacterState.Move;
                characterEntity.MoveBack();
                SendEntityEvent(EntityEvent.MoveBack);
            }
            rb.velocity = rb.velocity.y * Vector3.up + GameObjectTool.LogicUnitToWorld(characterEntity.direction) * (characterEntity.speed + 9.81f) / 100f;
        }
        else
        {
            //If not yet moving, then start
            if (motionState != CharacterState.Idle)
            {
                motionState = CharacterState.Idle;
                rb.velocity = Vector3.zero;
                characterEntity.Stop();
                SendEntityEvent(EntityEvent.Idle);
            }
        }

        if(Input.GetButtonDown("Jump"))
        {
            SendEntityEvent(EntityEvent.Jump);
        }

        float h = Input.GetAxis("Horizontal");
        if(h > 0.1 || h < -0.1)
        {
            transform.Rotate(0, h * rotateUnitAngle, 0);
            Vector3 dir = GameObjectTool.LogicUnitToWorld(characterEntity.direction);
            Quaternion rot = new Quaternion();
            rot.SetFromToRotation(dir, transform.forward);

            //Update our direction only when our current rotation exceeds turnAngle
            if(rot.eulerAngles.y > turnAngle && rot.eulerAngles.y < 360 - turnAngle)
            {
                characterEntity.SetDirection(GameObjectTool.WorldUnitToLogicInt(transform.forward));
                rb.transform.forward = transform.forward;
                SendEntityEvent(EntityEvent.None);
            }
        }
    }

    Vector3 lastPos;
    float lastSync;
    private void LateUpdate()
    {
        Vector3 distanceTraveled = rb.transform.position - lastPos;
        speed = (int)(distanceTraveled.magnitude * 100f / Time.deltaTime);
        lastPos = rb.transform.position;

        if((GameObjectTool.WorldUnitToLogicInt(rb.transform.position) - characterEntity.position).magnitude > 50)
        {
            characterEntity.SetPosition(GameObjectTool.WorldUnitToLogicInt(rb.transform.position));
            SendEntityEvent(EntityEvent.None);
        }
        transform.position = rb.transform.position;
    }

    private void SendEntityEvent(EntityEvent entityEvent)
    {
        if(entityController != null)
        {
            entityController.OnEntityEvent(entityEvent);
        }
        MapService.Instance.SendMapEntitySync(entityEvent, characterEntity.EntityData);
    }
}
