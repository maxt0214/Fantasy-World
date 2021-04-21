using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using Services;

public class UIMainCity : MonoBehaviour
{
    [Header("Player Avatar")]
    public Text playerName;
    public Text playerLevel;

    private void Start()
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
}
