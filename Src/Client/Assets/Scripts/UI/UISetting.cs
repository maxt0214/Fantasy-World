using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISetting : UIWindow
{

    public void OnClickSelectChara()
    {
        SceneManager.Instance.LoadScene("CharacterCreation");
        SoundManager.Instance.PlayMusic(SoundDefine.Music_Select);
        UserService.Instance.SendLeaveGame();
    }

    public void OnClickExitGame()
    {
        UserService.Instance.SendLeaveGame(true);
    }

    public void OnClickOpenConfig()
    {
        UIManager.Instance.Show<UISystemConfig>();
        Close();
    }
}
