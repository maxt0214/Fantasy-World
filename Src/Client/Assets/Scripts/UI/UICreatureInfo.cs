using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICreatureInfo : MonoBehaviour
{
    public Image Avatar;
    public Text Title;
    public Slider HpBar;
    public Text HpTxt;
    public Slider MpBar;
    public Text MpTxt;

    public UIBuffSlots buffSlots;

    private Creature target;
    public Creature Target
    {
        get { return target; }
        set
        {
            target = value;
            if(target != null) buffSlots.InitBuffSlots(target);
            InitUI();
        }
    }

    public int dropRate;
    private float dropAmt;
    //Refelecting real target Hp and Mp that we are intending to reach at some point
    private int targetHp;
    private int targetMp;
    //Current Hp and Mp that refelected on UI but not necessarily real ones
    private float currHp;
    private float currMp;

    private float dropTime;
    public float dropTimeInterval;

    private void Start()
    {
        dropTime = Time.time + dropTimeInterval;
    }

    public void InitUI()
    {
        dropAmt = target.Attributes.MaxHP * dropRate / 100;
        currHp = target.Attributes.HP;
        currMp = target.Attributes.MP;

        Title.text = target.Name;
        HpBar.maxValue = target.Attributes.MaxHP;
        HpBar.value = target.Attributes.HP;
        MpBar.maxValue = target.Attributes.MaxMP;
        MpBar.value = target.Attributes.MP;
        HpTxt.text = string.Format("{0}/{1}", target.Attributes.HP, target.Attributes.MaxHP);
        MpTxt.text = string.Format("{0}/{1}", target.Attributes.MP, target.Attributes.MaxMP);
    }

    public void UpdateUI()
    {
        if (target == null) return;

        targetHp = (int)target.Attributes.HP;
        targetMp = (int)target.Attributes.MP;
        Title.text = target.Name;

        if (Time.time < dropTime)
            return;
        else
            dropTime = Time.time + dropTimeInterval;

        if(targetHp != currHp)
        {
            currHp = (targetHp < currHp) ? Mathf.Clamp(currHp - dropAmt, targetHp, currHp) : Mathf.Clamp(currHp + dropAmt, currHp, targetHp);

            HpBar.maxValue = target.Attributes.MaxHP;
            HpBar.value = currHp;
            HpTxt.text = string.Format("{0}/{1}", currHp, target.Attributes.MaxHP);
        }

        if(targetMp != currMp)
        {
            currMp = (targetMp < currMp) ? Mathf.Clamp(currMp - dropAmt, targetMp, currMp) : Mathf.Clamp(currMp + dropAmt, currMp, targetMp);

            MpBar.maxValue = target.Attributes.MaxMP;
            MpBar.value = currMp;
            MpTxt.text = string.Format("{0}/{1}", currMp, target.Attributes.MaxMP);
        }
    }

    private void Update()
    {
        UpdateUI();
    }
}
