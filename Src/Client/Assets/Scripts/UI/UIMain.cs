using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using Services;
using Managers;

public class UIMain : MonoSingleton<UIMain>
{
    [Header("Player Avatar")]
    public Text playerName;
    public Text playerLevel;

    public UITeamView TeamWindow;

    protected override void OnStart()
    {
        UpdateAvatar();
    }

    private void UpdateAvatar()
    {
        playerName.text = User.Instance.CurrentCharacter.Name;
        playerLevel.text = User.Instance.CurrentCharacter.Level.ToString();
    }

    public void OnClickReturnToCharaSelect()
    {
        SceneManager.Instance.LoadScene("CharacterCreation");
        UserService.Instance.SendLeaveGame();
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

    }

    public void OnClickSetting()
    {

    }

    public void OnClickSkill()
    {

    }

    public void ShowTeamUI(bool ifShow)
    {
        TeamWindow.ShowTeam(ifShow);
    }
}
