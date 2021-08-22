using Battle;
using Common.Data;
using Models;
using UnityEngine;
using UnityEngine.UI;

public class UISkillItem : ListView.ListViewItem
{
    public Image skillIcon;
    public Text skillName;
    public Text skillLevel;

    public Image background;
    public Sprite select;
    public Sprite normal;

    public Skill skill;

    public override void OnSelected(bool ifSelected)
    {
        background.overrideSprite = ifSelected ? select : normal;
    }

    public void SetSkillInfo(Skill sk)
    {
        skill = sk;

        skillIcon.overrideSprite = Resloader.Load<Sprite>(skill.Def.Icon);
        skillName.text = skill.Def.Name;
        skillLevel.text = "Lv" + skill.Info.Lv.ToString();
    }
}
