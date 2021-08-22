using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using Services;
using Managers;
using Entities;
using System;

public class UIMain : MonoSingleton<UIMain>
{
    [Header("Player Avatar")]
    public Text playerName;
    public Text playerLevel;

    public UITeamView TeamWindow;

    public UICreatureInfo targetInfo;

    public UISKillSlots skillSlots;

    protected override void OnStart()
    {
        UpdateAvatar();
        targetInfo.gameObject.SetActive(false);
        BattleManager.Instance.OnTargetChanged += OnTargetChanged;
        User.Instance.OnCharacterInit += skillSlots.RefreshUI;
        skillSlots.RefreshUI();
    }

    private void UpdateAvatar()
    {
        playerName.text = User.Instance.CurrentCharacterInfo.Name;
        playerLevel.text = User.Instance.CurrentCharacterInfo.Level.ToString();
    }

    public void OnClickOpenBag()
    {
        UIManager.Instance.Show<UIBagView>();
    }

    public void OnClickOpenEquip()
    {
        UIManager.Instance.Show<UIEquip>();
    }

    public void OnClickOpenQuestSystem()
    {
        UIManager.Instance.Show<UIQuestSystem>();
    }

    public void OnClickOpenFriendSystem()
    {
        UIManager.Instance.Show<UIFriendView>();
    }

    public void OnClickGuild()
    {
        GuildManager.Instance.ShowGuild();
    }

    public void OnClickRide()
    {
        UIManager.Instance.Show<UIRide>();
    }

    public void OnClickSetting()
    {
        UIManager.Instance.Show<UISetting>();
    }

    public void OnClickSkill()
    {
        UIManager.Instance.Show<UISkillView>();
    }

    public void ShowTeamUI(bool ifShow)
    {
        TeamWindow.ShowTeam(ifShow);
    }

    private void OnTargetChanged(Creature target)
    {
        if (target != null)
        {
            if(!targetInfo.isActiveAndEnabled) targetInfo.gameObject.SetActive(true);
            targetInfo.Target = target;
        } else
        {
            targetInfo.gameObject.SetActive(false);
        }
    }
}
