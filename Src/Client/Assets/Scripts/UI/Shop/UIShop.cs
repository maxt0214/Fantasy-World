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
    public List<Transform> pages = new List<Transform>();
    public Transform pageTemplate;
    public Transform shopView;
    public GameObject shopItemEntry;

    public Text playerGold;
    public Text shopTitle;
    public Text pageInfo;

    private ShopDefine shop;
    private int currPage;
    private int itemCapacity;

    void Start()
    {
        currPage = 1;
        itemCapacity = 5;
        StartCoroutine(InitItems());
    }

    private IEnumerator InitItems()
    {
        int itemCount = 0;
        foreach(var kv in DataManager.Instance.ShopItems[shop.ID])
        {
            itemCount++;
            if (kv.Value.Status > 0)
            {
                if(currPage > pages.Count)
                {
                    var newPage = Instantiate(pageTemplate, shopView);
                    newPage.name = "Page" + currPage;
                    pages.Add(newPage);
                }
                var itemEntry = Instantiate(shopItemEntry, pages[currPage-1]);
                var uiShopItem = itemEntry.GetComponent<UIShopItem>();
                uiShopItem.SetShopItem(kv.Key,kv.Value,this);
            }
            if (itemCount >= itemCapacity)
            {
                itemCount = 0;
                currPage++;
            }
        }
        currPage = 1;
        OnClickSwitchPage(0);
        yield return null;
    }

    public void SetShop(ShopDefine shopDef)
    {
        shop = shopDef;
        playerGold.text = User.Instance.CurrentCharacterInfo.Gold.ToString();
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

    public void OnClickSwitchPage(int pageChange)
    {
        currPage = Mathf.Clamp(currPage+pageChange,1,pages.Count);
        pageInfo.text = currPage + " / " + pages.Count;
        for(int i = 0; i < pages.Count; i++)
        {
            pages[i].gameObject.SetActive(i == currPage-1);
        }
    }
}
