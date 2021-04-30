using Models;
using System.Collections.Generic;
using UnityEngine;
using Common.Data;
using Services;
using System;
using SkillBridge.Message;

namespace Managers
{
    class EquipManager : Singleton<EquipManager>
    {
        public delegate void OnEquipChangeHandler();
        public event OnEquipChangeHandler OnEquipChanged;

        public Item[] equipSlots = new Item[(int)EquipSlot.SlotCap];

        byte[] Data;

        unsafe public void Init(byte[] data)
        {
            Data = data;
            ParseEquipData(data);
        }

        unsafe void ParseEquipData(byte[] data)
        {
            fixed(byte* pt = data)
            {
                for(int i = 0; i < (int)EquipSlot.SlotCap; i++)
                {
                    int itemId = *(int*)(pt + i * sizeof(int));
                    if (itemId > 0)
                        equipSlots[i] = ItemManager.Instance.Items[itemId];
                    else
                        equipSlots[i] = null;
                }
            }
        }

        unsafe public byte[] GetEquipData()
        {
            fixed (byte* pt = Data)
            {
                for(int i = 0; i < (int)EquipSlot.SlotCap; i++)
                {
                    int* itemId = (int*)(pt + i * sizeof(int));
                    if (equipSlots[i] == null)
                        *itemId = 0;
                    else
                        *itemId = equipSlots[i].Id;
                }
            }
            return Data;
        }

        public bool ContainsId(int equipId)
        {
            for(int i = 0; i < (int)EquipSlot.SlotCap; i++)
            {
                if (equipSlots[i] != null && equipSlots[i].Id == equipId)
                    return true;
            }
            return false;
        }

        public Item GetEquip(EquipSlot slot)
        {
            return equipSlots[(int)slot];
        }

        public void EquipItem(Item equip)
        {
            ItemService.Instance.SendItemEquip(equip, true);
        }

        public void UnequipItem(Item equip)
        {
            ItemService.Instance.SendItemEquip(equip, false);
        }

        public void OnEquipItem(Item equip)
        {
            if(equipSlots[(int)equip.equipDef.Slot] != null && equipSlots[(int)equip.equipDef.Slot].Id == equip.Id)
            {
                return;
            }
            equipSlots[(int)equip.equipDef.Slot] = ItemManager.Instance.Items[equip.Id];

            if (OnEquipChanged != null)
                OnEquipChanged();
        }

        public void OnUnequipItem(EquipSlot slot)
        {
            if(equipSlots[(int)slot] != null)
            {
                equipSlots[(int)slot] = null;
                if (OnEquipChanged != null)
                    OnEquipChanged();
            }
        }
    }
}
