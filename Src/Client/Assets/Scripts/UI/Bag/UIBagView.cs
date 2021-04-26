using Managers;
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

    List<Image> slots;

    void Start()
    {
        highlighted = switchBagButton[0].sprite;
        SetUpBagView(0);

        if(slots == null)
        {
            slots = new List<Image>();
            for(int page = 0; page < pages.Length; page++)
            {
                slots.AddRange(pages[page].GetComponentsInChildren<Image>(true));
            }
        }
        StartCoroutine(InitBag());
    }

    IEnumerator InitBag()
    {
        for(int i = 0; i < BagManager.Instance.Items.Length; i++)
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

    public void SetUpBagView(int curr)
    {
        for(int i = 0; i < switchBagButton.Length; i++)
        {
            switchBagButton[i].overrideSprite = (i == curr) ? selected : highlighted;
            bags[i].SetActive(i == curr);
        }
    }

    public void SetGoldAmount(string amount)
    {
        goldAmount.text = amount;
    }

    public void OnReset()
    {
        BagManager.Instance.Reset();
    }
}
