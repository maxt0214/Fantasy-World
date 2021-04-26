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

        public Item(NItemInfo itemInfo)
        {
            Id = itemInfo.Id;
            Count = itemInfo.Count;
            itemDef = DataManager.Instance.Items[itemInfo.Id];
        }

        public override string ToString()
        {
            return string.Format("Item[{0}] Count:{1}", Id, Count);
        }
    }
}
