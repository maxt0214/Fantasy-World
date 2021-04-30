using Common.Data;
using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEquipItem : MonoBehaviour, IPointerClickHandler
{
    public Image equipIcon;
    public Text equipName;
    public Text equipLevel;
    public Text equipType;

    public Image background;
    public Sprite select;
    public Sprite normal;

    /// <summary>
    /// Can be item ID or equip slot id. Do not use this as itemId.
    /// </summary>
    public int equipId { get; set; }
    public bool equiped { get; set; }
    private UIEquip equipView;
    private Item equipItem;

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

    public void SetEquipItem(int id, Item item, UIEquip owner, bool equiped)
    {
        equipId = id;
        equipItem = item;
        equipView = owner;
        this.equiped = equiped;

        equipIcon.overrideSprite = Resloader.Load<Sprite>(item.itemDef.Icon);
        if (equipName != null) equipName.text = equipItem.equipDef.Name;
        if (equipLevel != null) equipLevel.text = equipItem.itemDef.Level.ToString();
        if (equipType != null) equipType.text = equipItem.equipDef.Slot.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(equiped)
        {
            UpEquip();
        } else
        {
            if (selected)
            {
                Equip();
                Selected = false;
            }
            else
                Selected = true;
        }
    }

    private void Equip()
    {
        var prompt = MessageBox.Show(string.Format("Do You Wanna Equip {0}?", equipItem.itemDef.Name), "Confirmation", MessageBoxType.Confirm);
        prompt.OnYes = () =>
        {
            var preEquip = EquipManager.Instance.GetEquip(equipItem.equipDef.Slot);
            if (preEquip != null)
            {
                var doubleConfirm = MessageBox.Show(string.Format("Do You Wanna Replace {0}?", preEquip.itemDef.Name), "Confirmation", MessageBoxType.Confirm);
                doubleConfirm.OnYes = () => { equipView.Equip(equipItem); };
            }
            else
                equipView.Equip(equipItem);
        };
    }

    private void UpEquip()
    {
        var prompt = MessageBox.Show(string.Format("Do You Wanna Unequip {0}?", equipItem.itemDef.Name), "Confirmation", MessageBoxType.Confirm);
        prompt.OnYes = () => { equipView.Unequip(equipItem); };
    }
}
