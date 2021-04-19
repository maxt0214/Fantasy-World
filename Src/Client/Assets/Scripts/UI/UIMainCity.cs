using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;

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
}
