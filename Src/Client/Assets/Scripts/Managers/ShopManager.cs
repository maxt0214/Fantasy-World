using Common.Data;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Managers
{
    class ShopManager : Singleton<ShopManager>
    {
        public void Init()
        {
            NPCManager.Instance.RegisterNPCEvent(NPCFunction.InvokeShop, OnOpenShop);
        }

        private bool OnOpenShop(NPCDefine npc)
        {
            ShowShop(npc.Param);
            return true;
        }

        private void ShowShop(int shopId)
        {
            ShopDefine shop;
            if(DataManager.Instance.Shops.TryGetValue(shopId, out shop))
            {
                UIShop uiShop = UIManager.Instance.Show<UIShop>();
                if (uiShop != null) uiShop.SetShop(shop);
            }
        }

        public bool PurchaseItem(int shopId, int shopItemId)
        {
            ItemService.Instance.SendItemPurchase(shopId, shopItemId);
            return true;
        }

        public void UpdateShop()
        {

        }
    }
}
