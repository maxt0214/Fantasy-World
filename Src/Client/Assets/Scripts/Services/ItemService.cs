using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkillBridge.Message;
using UnityEngine;
using Models;
using Managers;

namespace Services
{
    public class ItemService : Singleton<ItemService>, IDisposable
    {
        public ItemService()
        {
            MessageDistributer.Instance.Subscribe<ItemPurchaseResponse>(OnItemPurchase);
            MessageDistributer.Instance.Subscribe<ItemEquipResponse>(OnItemEquip);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ItemPurchaseResponse>(OnItemPurchase);
            MessageDistributer.Instance.Unsubscribe<ItemEquipResponse>(OnItemEquip);
        }

        public void SendItemPurchase(int shopId, int shopItemId)
        {
            Debug.LogFormat("ItemPurcahseRequest: Shop[{0}] ShopItemId[{1}]", shopId, shopItemId);

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.itemPurchase = new ItemPurchaseRequest();
            message.Request.itemPurchase.shopId = shopId;
            message.Request.itemPurchase.shopItemId = shopItemId;

            NetClient.Instance.SendMessage(message);
        }

        private void OnItemPurchase(object sender, ItemPurchaseResponse message)
        {
            MessageBox.Show("Purcahse Result: " + message.Result + '\n' + message.Errormsg, "Purchase Finished");
        }

        Item pendingEquip;
        bool ifEquip;
        public bool SendItemEquip(Item equip, bool ifEquip)
        {
            if (pendingEquip != null)
                return false;

            Debug.LogFormat("ItemEquipRequest: Equip[{0}]", equip.Id);
            pendingEquip = equip;
            this.ifEquip = ifEquip;

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.ItemEquip = new ItemEquipRequest();
            message.Request.ItemEquip.Slot = (int)equip.equipDef.Slot;
            message.Request.ItemEquip.itemId = equip.Id;
            message.Request.ItemEquip.ifEquip = ifEquip;

            NetClient.Instance.SendMessage(message);
            return true;
        }

        private void OnItemEquip(object sender, ItemEquipResponse response)
        {
            if(response.Result == Result.Success)
            {
                if(pendingEquip != null)
                {
                    if (ifEquip)
                        EquipManager.Instance.OnEquipItem(pendingEquip);
                    else
                        EquipManager.Instance.OnUnequipItem(pendingEquip.equipDef.Slot);
                    pendingEquip = null;
                }
            }
        }
    }
}
