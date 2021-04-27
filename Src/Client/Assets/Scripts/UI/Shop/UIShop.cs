using Common.Data;
using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : UIWindow
{
    public Transform[] pages;
    public GameObject shopItemEntry;

    public Text playerGold;
    public Text shopTitle;

    private ShopDefine shop;

    void Start()
    {
        StartCoroutine(InitItems());
    }

    private IEnumerator InitItems()
    {
        foreach(var kv in DataManager.Instance.ShopItems[shop.ID])
        {
            if(kv.Value.Status > 0)
            {
                var itemEntry = Instantiate(shopItemEntry, pages[0]);
                var uiShopItem = itemEntry.GetComponent<UIShopItem>();
                uiShopItem.SetShopItem(kv.Key,kv.Value,this);
            }
        }
        yield return null;
    }

    public void SetShop(ShopDefine shopDef)
    {
        shop = shopDef;
        playerGold.text = User.Instance.CurrentCharacter.Gold.ToString();
        shopTitle.text = shop.Name;
    }

    private UIShopItem selectedShopItem;
    public void SelectShopItem(UIShopItem uIShopItem)
    {
        if (selectedShopItem != null) selectedShopItem.Selected = false;
        selectedShopItem = uIShopItem;
    }

    public void OnClickPurchase()
    {
        if(selectedShopItem == null)
        {
            MessageBox.Show("Please Select An Item To Purchase");
            return;
        }
        ShopManager.Instance.PurchaseItem(shop.ID, selectedShopItem.shopItemId);
    }
}
