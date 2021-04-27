using Common.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIShopItem : MonoBehaviour, ISelectHandler
{
    public Image itemIcon;
    public Text itemName;
    public Text itemPrice;
    public Text itemCount;

    public Image background;
    public Sprite select;
    public Sprite normal;

    private bool selected;
    public bool Selected
    {
        get { return selected; }
        set
        {
            selected = value;
            background.overrideSprite = selected ? select : normal;
        }
    }

    public int shopItemId { get; set; }
    private UIShop shop;
    private ShopItemDefine shopItem;
    private ItemDefine item;

    public void SetShopItem(int id, ShopItemDefine shopItemDef, UIShop owner)
    {
        shopItemId = id;
        shop = owner;
        shopItem = shopItemDef;
        item = DataManager.Instance.Items[shopItem.ItemID];

        itemIcon.overrideSprite = Resloader.Load<Sprite>(item.Icon);
        itemName.text = item.Name;
        itemPrice.text = shopItem.Price.ToString();
        itemCount.text = "X" + shopItem.Count;
    }

    public void OnSelect(BaseEventData eventData)
    {
        Selected = true;
        shop.SelectShopItem(this);
    }
}
