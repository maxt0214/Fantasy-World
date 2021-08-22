using Battle;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SkillBridge.Message;
using Managers;
using Models;

public class UISkillSlot : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public Image cdMask;
    public Text cdTime;

    public Skill skill;

    private void Start()
    {
        cdMask.enabled = false;
        cdTime.enabled = false;
    }

    private void Update()
    {
        if (skill == null) return;

        if(skill.CD > 0)
        {
            if(!cdMask.enabled) cdMask.enabled = true;
            if (!cdTime.enabled) cdTime.enabled = true;
            cdMask.fillAmount = skill.CD / skill.Def.CD;
            cdTime.text = ((int)Math.Ceiling(skill.CD)).ToString();
        } else
        {
            cdMask.enabled = false;
            cdTime.enabled = false;
        }
    }

    public void SetSkillSlot(Skill sk)
    {
        skill = sk;
        if(icon != null)
        {
            icon.overrideSprite = Resloader.Load<Sprite>(skill.Def.Icon);
            icon.SetAllDirty();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (skill.Def.TargetType == Common.Battle.TargetType.Area)
            TargetSelector.ShowSelector(User.Instance.currentCharacter.position, skill.Def.Range, skill.Def.AOERange, OnPositionSelected);
        else
            CastSkill();
    }

    private void OnPositionSelected(Vector3 pos)
    {
        BattleManager.Instance.EffectLoc = GameObjectTool.WorldUnitToLogicN(pos);
        CastSkill();
    }

    private void CastSkill()
    {
        SkillResult res = skill.Castable(BattleManager.Instance.Target, BattleManager.Instance.EffectLoc);

        switch (res)
        {
            case SkillResult.CoolDown:
                MessageBox.Show("Skill Is Cooling Down!");
                return;
            case SkillResult.InvalidMp:
                MessageBox.Show("Insufficient MP");
                return;
            case SkillResult.InvalidTarget:
                MessageBox.Show("Invalid Target");
                return;
            case SkillResult.OutOfRange:
                MessageBox.Show("Target Out Of Range");
                return;
        }

        BattleManager.Instance.CastSkill(skill);
    }
}
