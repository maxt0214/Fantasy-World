using Models;
using System.Collections.Generic;
using SkillBridge.Message;
using UnityEngine;
using Common.Data;
using Services;
using System;

namespace Managers
{
    class ItemManager : Singleton<ItemManager>
    {
        public delegate void OnItemChange(int itemId);
        public event OnItemChange ItemChanged;

        public Dictionary<int, Item> Items = new Dictionary<int, Item>();

        public void Init(List<NItemInfo> list)
        {
            Items.Clear();
            foreach(var info in list)
            {
                var item = new Item(info);
                Items.Add(item.Id,item);
                Debug.LogFormat("ItemManager:Init[{0}]", item.Id);
                RideManager.Instance.AddRide(item);
            }
            StatusService.Instance.RegisterStatusNotify(StatusType.Item, OnItemNotify);
        }

        private bool OnItemNotify(NStatus status)
        {
            if(status.Action == StatusAction.Add)
            {
                AddItem(status.Id, status.Value);
            }
            else if(status.Action == StatusAction.Delete)
            {
                RemoveItem(status.Id, status.Value);
            }
            if (ItemChanged != null) ItemChanged(status.Id);
            return true;
        }

        private void AddItem(int id, int value)
        {
            Item item = null;
            if(Items.TryGetValue(id, out item))
            {
                item.Count += value;
            } else
            {
                item = new Item(id, value);
                Items.Add(id,item);
            }
            BagManager.Instance.AddItem(id,value);
            RideManager.Instance.AddRide(item);
        }

        private void RemoveItem(int id, int value)
        {
            if (!Items.ContainsKey(id)) return;

            var item = Items[id];
            if (item.Count < value) return;

            item.Count -= value;
            BagManager.Instance.RemoveItem(id,value);
            RideManager.Instance.RemoveRide(item);
        }

        public ItemDefine GetItem(int itemId)
        {
            return null;
        }

        public bool UseItem(int itemId)
        {
            return false;
        }

        public bool UseItem(ItemDefine item)
        {
            return false;
        }
    }
}
