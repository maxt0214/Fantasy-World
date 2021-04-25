using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    class Item
    {
        TCharacterItem dbItem;

        public int ItemID;
        public int ItemCount;

        public Item(TCharacterItem item)
        {
            dbItem = item;
            ItemID = item.ID;
            ItemCount = item.ItemCount;
        }

        public void Add(int count)
        {
            ItemCount += count;
            dbItem.ItemCount = ItemCount;
        }

        public void Remove(int count)
        {
            ItemCount -= count;
            dbItem.ItemCount = ItemCount;
        }

        public bool Use(int count = 1)
        {

            return false;
        }

        public override string ToString()
        {
            return string.Format("ID:{0} Count:{1}", ItemID, ItemCount);
        }
    }
}
