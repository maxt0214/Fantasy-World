using Models;
using SkillBridge.Message;
using System;
using UnityEngine.Events;

namespace Managers
{
    class BagManager : Singleton<BagManager>
    {
        public UnityAction<int> OnBagChanged;

        public int unlocked;
        public BagItem[] Items;

        NBagInfo bagInfo;

        unsafe public void Init(NBagInfo info)
        {
            bagInfo = info;
            unlocked = info.Unlocked;
            Items = new BagItem[unlocked];
            if(info.Items != null && info.Items.Length > unlocked)
            {
                Deserialize(info.Items);
            } 
            else
            {
                bagInfo.Items = new byte[sizeof(BagItem) * unlocked];
                Reset();
            }
        }

        public void Reset()
        {
            int i = 0;
            foreach(var kv in ItemManager.Instance.Items)
            {
                if(kv.Value.Count <= kv.Value.itemDef.StackLimit)
                {
                    Items[i].ItemId = (ushort)kv.Key;
                    Items[i].Count = (ushort)kv.Value.Count;
                } else
                {
                    int count = kv.Value.Count;
                    while(count > kv.Value.itemDef.StackLimit)
                    {
                        Items[i].ItemId = (ushort)kv.Key;
                        Items[i].Count = (ushort)kv.Value.itemDef.StackLimit;
                        count -= kv.Value.itemDef.StackLimit;
                        i++;
                    }
                    Items[i].ItemId = (ushort)kv.Key;
                    Items[i].Count = (ushort)count;
                }
                i++;
            }
        }

        unsafe void Deserialize(byte[] items)
        {
            fixed(byte* pt = items)
            {
                for(int i = 0; i < unlocked; i++)
                {
                    BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));
                    Items[i] = *item;
                }
            }
        }

        unsafe public NBagInfo Serialize()
        {
            fixed (byte* pt = bagInfo.Items)
            {
                for (int i = 0; i < unlocked; i++)
                {
                    BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));
                    *item = Items[i];
                }
            }
            return bagInfo;
        }

        public void AddItem(int id, int value)
        {
            ushort countToAdd = (ushort)value, limit = (ushort)DataManager.Instance.Items[id].StackLimit;
            for(int i = 0; i < Items.Length; i++)
            {
                if(Items[i].ItemId == id)
                {
                    ushort gap = (ushort)(limit - Items[i].Count);
                    if(countToAdd > gap)
                    {
                        Items[i].Count += gap;
                        countToAdd -= gap;
                    } 
                    else
                    {
                        Items[i].Count += countToAdd;
                        countToAdd = 0;
                        break;
                    }
                }
            }

            if(countToAdd > 0)
            {
                for(int i = 0; i < Items.Length; i++)
                {
                    if(Items[i].ItemId == 0)
                    {
                        if(countToAdd > limit)
                        {
                            Items[i].ItemId = (ushort)id;
                            Items[i].Count = limit;
                            countToAdd -= limit;
                        } 
                        else
                        {
                            Items[i].ItemId = (ushort)id;
                            Items[i].Count = countToAdd;
                            break;
                        }
                    }
                }
            }
            if (OnBagChanged != null) OnBagChanged(id);
        }

        public void RemoveItem(int id, int value)
        {

            if (OnBagChanged != null) OnBagChanged(id);
        }
    }
}
