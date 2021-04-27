using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIIconItem : MonoBehaviour
{
    public Image mainImage;
    public Text count;
    public Image secondImage;

    public void SetIcon(string imageSource, string itemCount)
    {
        mainImage.overrideSprite = Resloader.Load<Sprite>(imageSource);
        count.text = itemCount;
    }
}
