using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkillBridge.Message;
using UnityEngine;

namespace Services
{
    public class ItemService : Singleton<ItemService>, IDisposable
    {
        public ItemService()
        {
            MessageDistributer.Instance.Subscribe<ItemPurchaseResponse>(OnItemPurchase);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ItemPurchaseResponse>(OnItemPurchase);
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
    }
}
