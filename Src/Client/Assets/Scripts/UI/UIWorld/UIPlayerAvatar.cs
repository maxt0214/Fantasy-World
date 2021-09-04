using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerAvatar : MonoBehaviour
{
    public Text playerName;
    public Text playerLevel;
    public Text playerHp;
    public Text playerMp;

    void Start()
    {
        User.Instance.OnCharacterInit += InitAvatar;
    }

    private void OnDestroy()
    {
        User.Instance.OnCharacterInit -= InitAvatar;
    }

    private void InitAvatar()
    {
        var currPlayer = User.Instance.currentCharacter;
        playerName.text = currPlayer.Name;
        playerLevel.text = currPlayer.Info.Level.ToString();
        playerHp.text = string.Format("{0}/{1}", currPlayer.Attributes.HP, currPlayer.Attributes.MaxHP);
        playerMp.text = string.Format("{0}/{1}", currPlayer.Attributes.MP, currPlayer.Attributes.MaxMP);
    }
}
