using Models;
using System.Collections.Generic;
using SkillBridge.Message;
using UnityEngine;
using Common.Data;

namespace Managers
{
    class ItemManager : Singleton<ItemManager>
    {
        public Dictionary<int, Item> Items = new Dictionary<int, Item>();

        internal void Init(List<NItemInfo> list)
        {
            Items.Clear();
            foreach(var info in list)
            {
                var item = new Item(info);
                Items.Add(item.Id,item);
                Debug.LogFormat("ItemManager:Init[{0}]", item);
            }
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
