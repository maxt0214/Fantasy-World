using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public enum EffectType
{
   None = 0,
   Bullet = 1,
   Position = 2,
   Hit = 3
}

public class EffectController : MonoBehaviour
{
    public float lifeSpan = 1f;
    private float time = 0;

    private EffectType type;
    private Transform target;

    private Vector3 targetPos;
    private Vector3 startPos;
    private Vector3 offset;

    private void OnEnable()
    {
        if (type != EffectType.Bullet)
        {
            StartCoroutine(Run());
        }
    }

    private IEnumerator Run()
    {
        yield return new WaitForSeconds(lifeSpan);
        gameObject.SetActive(false);
    }

    public void Init(EffectType type, Transform source, Transform target, Vector3 offset, float duration)
    {
        this.type = type;
        this.target = target;
        if(duration > 0)
            lifeSpan = duration;
        time = 0;
        if(type == EffectType.Bullet)
        {
            startPos = transform.position;
            this.offset = offset;
            targetPos = this.target.position + this.offset;
        } else if(type == EffectType.Hit)
        {
            transform.position = target.position + offset;
        }
    }

    private void Update()
    {
        if(type == EffectType.Bullet)
        {
            time += Time.deltaTime;
            if(target != null)
            {
                targetPos = target.position + offset;
            }
            transform.LookAt(targetPos);
            if(Vector3.Distance(targetPos,transform.position) < 0.5f)
            {
                Destroy(gameObject);
                return;
            }
            if(lifeSpan > 0 && time > lifeSpan)
            {
                Destroy(gameObject);
                return;
            }
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime / (lifeSpan - time));
        }
    }
}
