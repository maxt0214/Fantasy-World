using Common;
using GameServer.Entities;
using GameServer.Models;
using System.Collections.Generic;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class ItemManager
    {
        Character Owner;
        public Dictionary<int, Item> Items = new Dictionary<int, Item>();

        public ItemManager(Character owner)
        {
            Owner = owner;
            foreach(var item in owner.Data.Items)
            {
                Items.Add(item.ItemID,new Item(item));
            }
        }

        public bool UserItem(int itemId, int count = 1)
        {
            Log.InfoFormat("[{0}] used {2} of Item:[{1}]", Owner.Data.ID, itemId, count);
            Item item = null;
            if(Items.TryGetValue(itemId,out item))
            {
                if(item.ItemCount < count)
                {
                    return false;
                }
                //TODO: Actually Use Item
                item.Remove(count);
                return true;
            }
            return false;
        }

        public bool HasItem(int itemId)
        {
            Item item = null;
            if (Items.TryGetValue(itemId, out item))
                return item.ItemCount > 0;

            return false;
        }

        public Item GetItem(int itemId)
        {
            Item item = null;
            Items.TryGetValue(itemId, out item);
            Log.InfoFormat("[{0}] GetItem[{1}] : {2}", Owner.Data.ID, itemId, item);
            return item;
        }

        public bool AddItem(int itemId, int count)
        {
            Item item = null;
            if(Items.TryGetValue(itemId, out item))
            {
                item.Add(count);
            } else
            {
                TCharacterItem dbItem = new TCharacterItem();
                dbItem.CharacterID = Owner.Data.ID;
                dbItem.Owner = Owner.Data;
                dbItem.ItemID = itemId;
                dbItem.ItemCount = count;
                Owner.Data.Items.Add(dbItem);
                item = new Item(dbItem);
                Items.Add(itemId,item);
            }
            Owner.StatusManager.ChangeItemStatus(itemId, count, StatusAction.Add);
            Log.InfoFormat("Character[{0}] AddItem[{1}] CountAdded:{2}", Owner.Data.ID, itemId, count);
            //DBService.Instance.Save();
            return true;
        }

        public bool RemoveItem(int itemId, int count)
        {
            Item item = null;
            if (!Items.TryGetValue(itemId, out item))
                return false;

            if (item.ItemCount < count)
                return false;

            item.Remove(count);
            Owner.StatusManager.ChangeItemStatus(itemId, count, StatusAction.Delete);
            Log.InfoFormat("[{0}] RemoveItem[{1}] CountRemoved:{2}", Owner.Data.ID, itemId, count);
            //DBService.Instance.Save();
            return true;
        }

        public void GetItemInfos(List<NItemInfo> list)
        {
            foreach(var item in Items)
            {
                list.Add(new NItemInfo { Id = item.Value.ItemID, Count = item.Value.ItemCount });
            }
        }
    }
}
