using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputBox
{
    static Object cacheObject = null;

    public static UIInputBox Show(string title, string message, string btnOK = "", string btnCancel = "", string emptyPrompt = "")
    {
        if (cacheObject == null)
        {
            cacheObject = Resloader.Load<Object>("UI/UIInputBox");
        }

        GameObject go = (GameObject)GameObject.Instantiate(cacheObject);
        UIInputBox inputbox = go.GetComponent<UIInputBox>();
        inputbox.Init(title, message, btnOK, btnCancel, emptyPrompt);
        return inputbox;
    }
}
