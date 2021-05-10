using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIIconItem : MonoBehaviour
{
    public Image mainImage;
    public Text count;
    public Image secondImage;

    private Sprite defaultImg;
    private string defaultCount;
    private void Start()
    {
        defaultImg = mainImage.sprite;
        defaultCount = count.text;
    }

    public void SetIcon(string imageSource, string itemCount)
    {
        mainImage.overrideSprite = Resloader.Load<Sprite>(imageSource);
        count.text = itemCount;
    }

    public void ResetIcon()
    {
        mainImage.overrideSprite = defaultImg;
        count.text = defaultCount;
    }
}
