using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using UnityEngine.UI;

public class UINameBar : MonoBehaviour
{
    public Text playerInfo;
    public Character character;

    void Start()
    {
        if(character != null)
        {
            UpdatePlayerInfo();
        }
    }

    private void Update()
    {
        UpdatePlayerInfo();
    }

    private void UpdatePlayerInfo()
    {
        if(character != null)
        {
            string info = character.Name + " Lv." + character.Info.Level;
            if(info != playerInfo.text)
            {
                playerInfo.text = info;
            }
        }
    }
}
