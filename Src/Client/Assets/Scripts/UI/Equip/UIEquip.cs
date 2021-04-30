using Common.Data;
using Managers;
using Models;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEquip : UIWindow
{
    [Header("Equip Item List")]
    public Transform equipListView;
    public GameObject equipItemPrefab;
    [Header("Equip Slot Icon")]
    public GameObject equipedItemPrefab;
    public Transform[] equipSlots;

    private void Start()
    {
        RefreshUI();
        EquipManager.Instance.OnEquipChanged += RefreshUI;
    }

    private void OnDestroy()
    {
        EquipManager.Instance.OnEquipChanged -= RefreshUI;
    }

    private void RefreshUI()
    {
        ClearEquipList();
        InitEquipList();
        ClearEquipedList();
        InitEquipedList();
    }

    private void ClearEquipList()
    {
        foreach (var item in equipListView.GetComponentsInChildren<UIEquipItem>())
        {
            Destroy(item.gameObject);
        }
    }

    private void InitEquipList()
    {
        foreach(var kv in ItemManager.Instance.Items)
        {
            if(kv.Value.itemDef.Type == ItemType.Equip && kv.Value.itemDef.Class == User.Instance.CurrentCharacter.Class)
            {
                if (EquipManager.Instance.ContainsId(kv.Key))
                    continue;
                var equipEntry = Instantiate(equipItemPrefab, equipListView);
                var equipItem = equipEntry.GetComponent<UIEquipItem>();
                equipItem.SetEquipItem(kv.Key,kv.Value,this,false);
            }
        }
    }

    private void ClearEquipedList()
    {
        foreach (var item in equipSlots)
        {
            if (item.childCount > 0)
                Destroy(item.GetChild(0).gameObject);
        }
    }

    private void InitEquipedList()
    {
        for(int i = 0; i < (int)EquipSlot.SlotCap; i++)
        {
            var item = EquipManager.Instance.equipSlots[i];
            if (item != null)
            {
                var equipEntry = Instantiate(equipedItemPrefab, equipSlots[i]);
                var equipItem = equipEntry.GetComponent<UIEquipItem>();
                equipItem.SetEquipItem(i, item, this, true);
            }
        }
    }

    public void Equip(Item item)
    {
        EquipManager.Instance.EquipItem(item);
    }

    public void Unequip(Item item)
    {
        EquipManager.Instance.UnequipItem(item);
    }
}
