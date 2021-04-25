using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITestingWindow : UIWindow
{
    public Text title;

    public void SetTitle(string titleTxt)
    {
        title.text = titleTxt;
    }
}
