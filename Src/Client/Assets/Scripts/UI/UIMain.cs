using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using Services;

public class UIMain : MonoSingleton<UIMain>
{
    [Header("Player Avatar")]
    public Text playerName;
    public Text playerLevel;

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
}
