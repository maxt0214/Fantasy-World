using Models;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class UIRideItem : ListView.ListViewItem
{
    public Image icon;
    public Text rideName;
    public Text level;

    public Item ride;

    public Image background;
    public Sprite select;
    public Sprite normal;

    public override void OnSelected(bool ifSelected)
    {
        background.overrideSprite = ifSelected ? select : normal;
    }

    public void SetRideItemInfo(Item item, UIRide Owner, bool equiped)
    {
        ride = item;

        rideName.text = item.itemDef.Name;
        level.text = "Level:" + item.itemDef.Level;
        icon.overrideSprite = Resloader.Load<Sprite>(item.itemDef.Icon);
    }
}
