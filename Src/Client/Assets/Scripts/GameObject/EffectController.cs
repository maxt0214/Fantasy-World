using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

 public enum EffectType
 {
    None = 0,
    Bullet = 1
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

    public void Init(EffectType type, Transform source, Transform target, float duration)
    {
        this.type = type;
        this.target = target;
        lifeSpan = duration;
        if(type == EffectType.Bullet)
        {
            startPos = transform.position;
            offset = new Vector3(0, transform.position.y - source.position.y, 0);
            targetPos = this.target.position + offset;
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
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime / (lifeSpan - time));
        }
    }
}
