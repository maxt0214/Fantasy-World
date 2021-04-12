using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.Data;
using System.Linq;

public class UICharacterCreateView : MonoBehaviour {
    public UICharacterView charaView;
    [Header("Character Description")]
    public GameObject[] titles;
    public Text description;

    [Header("Character Classes")]
    public Text[] characterClasses;
    public Text nickName;

    private void Start()
    {
        DataManager.Instance.Load();
        for(int i = 0; i < characterClasses.Length; i++)
        {
            characterClasses[i].text = DataManager.Instance.Characters.Values.Where(c => c.TID == i+1).FirstOrDefault().Name;
        }
    }

    public void OnClickSwitchCharacter(int charaIndex)
    {
        charaView.currChar = charaIndex;
        for(int i = 0; i < titles.Length; i++)
        {
            titles[i].SetActive(i == charaIndex);
        }
    }

    public void OnClickStartAdventure()
    {
        if(string.IsNullOrEmpty(nickName.text))
        {
            MessageBox.Show("Please Enter Your Nick Name");
            return;
        }

        if(nickName.text[0] == ' ')
        {
            MessageBox.Show("Your Nick Name Cannot Start With A Space");
            return;
        }

        Debug.Log("Character created!");
    }
}
