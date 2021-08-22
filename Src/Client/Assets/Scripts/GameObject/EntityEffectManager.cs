using Entities;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityEffectManager : MonoBehaviour
{
    public Transform Root;

    private Dictionary<string, GameObject> effects = new Dictionary<string, GameObject>();

    public Transform[] Props;

    void Start()
    {
        effects.Clear();
        
        for(int i = 0; i < Root.childCount; i++)
        {
            effects[Root.GetChild(i).name] = Root.GetChild(i).gameObject;
        }

        for (int i = 0; i < Props.Length; i++)
        {
            effects[Props[i].name] = Props[i].gameObject;
        }
    }

    public void PlayEffect(string name)
    {
        Debug.LogFormat("PlayEffect:{0} Effect:{1}", this.name,name);
        if(effects.ContainsKey(name))
        {
            effects[name].SetActive(true);
        }
    }

    public void PlayEffect(EffectType type, string name, Transform target, float duration)
    {
        if (type == EffectType.Bullet)
        {
            EffectController effect = InstantiateEffect(name);
            effect.Init(type, transform, target, duration);
            effect.gameObject.SetActive(true);
        }
        else
            PlayEffect(name);
    }

    private EffectController InstantiateEffect(string name)
    {
        GameObject prefab;
        if(effects.TryGetValue(name, out prefab))
        {
            GameObject go = Instantiate(prefab, GameObjectManager.Instance.transform, true);
            go.transform.position = prefab.transform.position;
            go.transform.rotation = prefab.transform.rotation;
            return go.GetComponent<EffectController>();
        }
        return null;
    }
}
