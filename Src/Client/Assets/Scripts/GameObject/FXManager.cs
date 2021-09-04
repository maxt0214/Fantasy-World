using Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoSingleton<FXManager>
{
    public GameObject[] Prefabs;
    private Dictionary<string, GameObject> effects = new Dictionary<string, GameObject>();

    protected override void OnStart()
    {
        for(int i = 0; i < Prefabs.Length; i++)
        {
            Prefabs[i].SetActive(false);
            effects[Prefabs[i].name] = Prefabs[i];
        }
    }

    private EffectController CreateEffect(string name, Vector3 pos)
    {
        GameObject prefab;
        if(effects.TryGetValue(name, out prefab))
        {
            GameObject go = Instantiate(prefab, transform, true);
            go.transform.position = pos;
            return go.GetComponent<EffectController>();
        }
        return null;
    }

    public void PlayEffect(EffectType type, string name, Transform target, Vector3 offset, float duration)
    {
        var effect = CreateEffect(name,offset);
        if (effect == null)
        {
            Debug.LogErrorFormat("Effect:{0} does not exist", name);
            return;
        }
        effect.Init(type,transform,target,offset,duration);
        effect.gameObject.SetActive(true);
    }
}
