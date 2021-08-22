using Battle;
using Common.Data;
using Models;
using SkillBridge.Message;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UISkillInfo : MonoBehaviour
{
    public Text title;
    public Text level;
    public Text maxLv;
    public Text description;
    public Text spCost;
    public Text goldCost;

    public Skill skill;

    public void SetSkillInfo(Skill sk)
    {
        skill = sk;

        title.text = skill.Def.Name;
        level.text = "Lv " + skill.Info.Lv.ToString();
        maxLv.text = "Max Lv " + skill.Def.Level.ToString();
        description.text = skill.Def.Description;

    }
}
