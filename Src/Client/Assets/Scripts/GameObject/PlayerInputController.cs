using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Entities;
using SkillBridge.Message;
using Services;
using Managers;
using UnityEngine.Events;

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

    private NavMeshAgent agent;
    private bool autoNav = false;

    public UnityAction NavagationOver;

    private void Start()
    {
        motionState = CharacterState.Idle;

        if(agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
            agent.stoppingDistance = 0.5f;
        }
    }

    public void StartNav(Vector3 destiny, float offset = 3f)
    {
        agent.stoppingDistance = offset;
        StartCoroutine(BeginNavagation(destiny));
    }

    private IEnumerator BeginNavagation(Vector3 destiny)
    {
        agent.SetDestination(destiny);
        yield return null;
        autoNav = true;
        if(motionState != CharacterState.Move)
        {
            motionState = CharacterState.Move;
            characterEntity.MoveForward();
            SendEntityEvent(EntityEvent.MoveFwd);
            agent.speed = characterEntity.speed / 100f;
        }
    }

    public void StopNav()
    {
        if (autoNav == false) return;
        autoNav = false;
        agent.ResetPath();
        if (motionState != CharacterState.Idle)
        {
            motionState = CharacterState.Idle;
            rb.velocity = Vector3.zero;
            characterEntity.Stop();
            SendEntityEvent(EntityEvent.Idle);
        }
        NavPathRenderer.Instance.SetPath(null, Vector3.zero);

        if (NavagationOver != null)
        {
            NavagationOver();
            NavagationOver = null;
        }
    }

    public void NavMove()
    {
        if(Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Input.GetButtonDown("Jump"))
        {
            StopNav();
            return;
        }

        if(agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            StopNav();
            return;
        }

        NavPathRenderer.Instance.SetPath(agent.path, agent.destination);
        if(agent.isStopped || agent.remainingDistance < 3f)
        {
            StopNav();
            return;
        }
    }

    private void FixedUpdate()
    {
        if (characterEntity == null) return;

        if(autoNav)
        {
            NavMove();
            return;
        }

        if (InputManager.Instance != null && InputManager.Instance.InputMode) return;

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
        if (characterEntity == null) return;

        //Sync Position To Remote
        Vector3 distanceTraveled = rb.transform.position - lastPos;
        speed = (int)(distanceTraveled.magnitude * 100f / Time.deltaTime);
        lastPos = rb.transform.position;

        if((GameObjectTool.WorldUnitToLogicInt(rb.transform.position) - characterEntity.position).magnitude > 50)
        {
            characterEntity.SetPosition(GameObjectTool.WorldUnitToLogicInt(rb.transform.position));
            SendEntityEvent(EntityEvent.None);
        }
        transform.position = rb.transform.position;
        
        //Sync Direction To Remote
        Vector3 dir = GameObjectTool.LogicUnitToWorld(characterEntity.direction);
        Quaternion rot = new Quaternion();
        rot.SetFromToRotation(dir, transform.forward);

        //Update our direction only when our current rotation exceeds turnAngle
        if (rot.eulerAngles.y > turnAngle && rot.eulerAngles.y < 360 - turnAngle)
        {
            characterEntity.SetDirection(GameObjectTool.WorldUnitToLogicInt(transform.forward));
            SendEntityEvent(EntityEvent.None);
        }
    }

    public void SendEntityEvent(EntityEvent entityEvent, int param = 0)
    {
        if(entityController != null)
        {
            entityController.OnEntityEvent(entityEvent, param);
        }
        MapService.Instance.SendMapEntitySync(entityEvent, characterEntity.EntityData, param);
    }
}
