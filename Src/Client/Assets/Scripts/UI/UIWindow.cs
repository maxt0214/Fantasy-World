using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIWindow : MonoBehaviour
{
    public delegate void CloseHandler(UIWindow sender, WindowResult result);
    public event CloseHandler OnClose;

    public virtual System.Type Type { get { return GetType(); } }

    public enum WindowResult { None = 0, Confirm = 1, No = 2 }

    public void Close(WindowResult result = WindowResult.None)
    {
        UIManager.Instance.Close(Type);
        if(OnClose != null)
            OnClose(this,result);
        OnClose = null;
    }

    public virtual void OnClickClose()
    {
        Close();
    }

    public virtual void OnClickConfirm()
    {
        Close(WindowResult.Confirm);
    }
}
