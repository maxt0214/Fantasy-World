using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBagView : UIWindow
{
    [Header("Bag View Switching")]
    public Image[] switchBagButton;
    public GameObject[] bags;
    public Sprite selected;
    private Sprite highlighted;

    [Header("Display Info")]
    public Transform[] pages;
    public Text goldAmount;
    public GameObject bagItem;

    List<Image> slots = new List<Image>();

    private void Start()
    {
        highlighted = switchBagButton[0].sprite;
        SwitchBagView(0);

        if(slots.Count == 0)
        {
            for(int page = 0; page < pages.Length; page++)
            {
                slots.AddRange(pages[page].GetComponentsInChildren<Image>(true));
            }
        }
        StartCoroutine(InitBag());
    }

    IEnumerator InitBag()
    {
        goldAmount.text = User.Instance.CurrentCharacter.Gold.ToString();

        for (int i = 0; i < BagManager.Instance.Items.Length; i++)
        {
            var item = BagManager.Instance.Items[i];
            if(item.ItemId > 0)
            {
                var itemObj = Instantiate(bagItem, slots[i].transform);
                var itemIcon = itemObj.GetComponent<UIIconItem>();
                var define = ItemManager.Instance.Items[item.ItemId].itemDef;
                itemIcon.SetIcon(define.Icon, item.Count.ToString());
            }
        }
        for(int i = BagManager.Instance.Items.Length; i < slots.Count; i++)
        {
            slots[i].color = Color.gray;
        }
        yield return null;
    }

    public void SwitchBagView(int curr)
    {
        for(int i = 0; i < switchBagButton.Length; i++)
        {
            switchBagButton[i].overrideSprite = (i == curr) ? selected : highlighted;
            bags[i].SetActive(i == curr);
        }
    }

    public void Clear()
    {
        for(int i = 0; i < slots.Count; i++)
        {
            if(slots[i].transform.childCount > 0)
            {
                Destroy(slots[i].transform.GetChild(0).gameObject);
            }
        }
    }

    public void OnReset()
    {
        BagManager.Instance.Reset();
        Clear();
        StartCoroutine(InitBag());
    }
}
