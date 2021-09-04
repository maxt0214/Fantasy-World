using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PopUpType
{
    None = 0,
    Dmg = 1,
    Heal = 2
}

public class UIPopUpText : MonoBehaviour
{
    public Text normalDmg;
    public Text critDmg;
    public Text heal;
    public float floatingTime = 0.5f;

    public void InitPopUp(PopUpType type, float val, bool ifCrit)
    {
        string text = val.ToString("0");
        normalDmg.text = text;
        critDmg.text = text;
        heal.text = text;

        normalDmg.enabled = val < 0 && !ifCrit;
        critDmg.enabled = val < 0 && ifCrit;
        heal.enabled = val > 0;

        float time = Random.Range(0f, 0.5f) + floatingTime;

        float height = Random.Range(0.5f,1f);
        float disperse = Random.Range(-0.5f,0.5f);
        disperse += Mathf.Sign(disperse) * 0.3f;

        LeanTween.moveX(gameObject, transform.position.x + disperse, time);
        LeanTween.moveZ(gameObject, transform.position.z + disperse, time);
        LeanTween.moveY(gameObject, transform.position.y + height, time).setEaseOutBack().setDestroyOnComplete(true);
    }
}
