using Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBuffIcon : MonoBehaviour
{
    public Image Icon;

    private Buff buff;

    public void SetBuffIcon(Buff bf)
    {
        buff = bf;
        if(Icon != null)
        {
            Icon.overrideSprite = Resloader.Load<Sprite>(buff.Def.Icon);
        }
    }
}
