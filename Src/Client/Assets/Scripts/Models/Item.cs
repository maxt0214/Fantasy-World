using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Data;
using SkillBridge.Message;

namespace Models
{
    class Item
    {
        public int Id;
        public int Count;
        public ItemDefine itemDef;

        public Item(NItemInfo itemInfo) : this(itemInfo.Id, itemInfo.Count) {}

        public Item(int id, int count)
        {
            Id = id;
            Count = count;
            itemDef = DataManager.Instance.Items[Id];
        }

        public override string ToString()
        {
            return string.Format("Item[{0}] Count:{1}", Id, Count);
        }
    }
}
