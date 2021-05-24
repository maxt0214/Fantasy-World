using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISetting : UIWindow
{

    public void OnClickSelectChara()
    {
        SceneManager.Instance.LoadScene("CharacterCreation");
        UserService.Instance.SendLeaveGame();
    }

    public void OnClickExitGame()
    {
        UserService.Instance.SendLeaveGame(true);
    }
}
