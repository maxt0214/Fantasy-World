using Battle;
using Entities;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBuffSlots : MonoBehaviour
{
    public Creature Owner;
    public GameObject prefab;
    private Dictionary<int,GameObject> buffItems = new Dictionary<int, GameObject>();

    private void Awake()
    {
        prefab.SetActive(false);
    }

    private void OnDestroy()
    {
        Clear();
    }

    public void InitBuffSlots(Creature owner)
    {
        if (Owner == owner) return;
        if(owner != null && owner != Owner)
        {
            Clear();
        }
        Owner = owner;
        Owner.OnBuffChanged += BuffChanged;
        InitBuffS();
    }

    private void BuffChanged(Buff buff, BuffAction action)
    {
        if(action == BuffAction.Add)
        {
            var go = Instantiate(prefab, transform);
            go.name = string.Format("{0}_{1}", buff.Def.Name, buff.ID);
            UIBuffIcon buffItem = go.GetComponent<UIBuffIcon>();
            buffItem.SetBuffIcon(buff);
            go.SetActive(true);
            buffItems[buff.ID] = go;
        } else if(action == BuffAction.Remove)
        {
            GameObject go;
            if(buffItems.TryGetValue(buff.ID,out go))
            {
                buffItems.Remove(buff.ID);
                Destroy(go);
            }
        }
    }

    private void Clear()
    {
        if(Owner != null)
        {
            Owner.OnBuffChanged -= BuffChanged;
        }

        foreach(var go in buffItems)
        {
            Destroy(go.Value);
        }
        buffItems.Clear();
    }

    private void InitBuffS()
    {
        foreach(var buff in Owner.buffMgr.buffs)
        {
            BuffChanged(buff.Value,BuffAction.Add);
        }
    }
}
