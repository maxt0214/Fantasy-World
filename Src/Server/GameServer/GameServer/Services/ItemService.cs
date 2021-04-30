using System;
using System.Collections.Generic;
using Common;
using Network;
using SkillBridge.Message;
using GameServer.Entities;
using GameServer.Managers;

namespace GameServer.Services
{
    class ItemService : Singleton<ItemService>
    {
        public ItemService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ItemPurchaseRequest>(OnItemPurchase);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ItemEquipRequest>(OnItemEquip);
        }

        public void Init()
        {

        }

        private void OnItemPurchase(NetConnection<NetSession> sender, ItemPurchaseRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("ItemPurchaseRequest: Character:{0} ShopId:{1} ItemId:{2}", character.Id, request.shopId, request.shopItemId);
            var result = ShopManager.Instance.PurchaseItem(sender, request.shopId, request.shopItemId);
            sender.Session.Response.itemPurchase = new ItemPurchaseResponse();
            sender.Session.Response.itemPurchase.Result = result;
            sender.SendResponse();
        }

        private void OnItemEquip(NetConnection<NetSession> sender, ItemEquipRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("ItemEquipRequest: Character:{0} Slot:{1} ItemId:{2} IfToEquip:{3}", character.Id, request.Slot, request.itemId, request.ifEquip);
            var result = EquipManager.Instance.EquipItem(sender, request.Slot, request.itemId, request.ifEquip);
            sender.Session.Response.ItemEquip = new ItemEquipResponse();
            sender.Session.Response.ItemEquip.Result = result;
            sender.SendResponse();
        }
    }
}
