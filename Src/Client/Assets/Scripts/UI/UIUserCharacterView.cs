﻿using UnityEngine;
using Models;
using Services;
using SkillBridge.Message;

public class UIUserCharacterView : MonoSingleton<UIUserCharacterView>
{
    public GameObject charaSelectPanel;
    public GameObject charaCreatePanel;
    public UICharacterSelectionView selectView;
    public UICharacterCreateView createView;

    public UICharacterView charaView;
    public int characterCap = 5;

    void Start()
    {
        //Show the proper panel
        charaSelectPanel.SetActive(true);
        charaCreatePanel.SetActive(false);

        //initiate select view
        if (User.Instance.Info == null) //in case of local debug
            selectView.UpdateCharactersLocal();
        else //Connected to network and update characters accordingly
            selectView.UpdateCharacters(characterCap);

        //Initiate create view
        createView.Init();
    }

    private void OnEnable()
    {
        UserService.Instance.OnCreateCharacter += OnCreateCharacter;
    }

    private void OnDisable()
    {
        UserService.Instance.OnCreateCharacter -= OnCreateCharacter;
    }

    public void OnCharacterEntrySelected(int selected)
    {
        selectView.currSelected = selected;

        var currCharaClass = selectView.currEntry.currClass;
        if (currCharaClass == CharacterClass.None) //Empty character. Turn to create panel
        {
            charaSelectPanel.SetActive(false);
            charaCreatePanel.SetActive(true);
        } else
        {
            charaView.currChar = (int)currCharaClass;
        }
    }

    public void OnClickStartAdventure()
    {

    }

    #region Character Create Funcs
    public void OnClickSwitchCharacterPreview(int charaClass)
    {
        int charaIndex = charaClass - 1;

        charaView.currChar = charaClass;
        createView.SwitchCharacter(charaIndex, (CharacterClass)charaClass);
    }

    public void OnClickCreateCharacter()
    {
        createView.CreateCharacter();
    }

    public void OnCreateCharacter(Result res, string errMsg)
    {
        if (res == Result.Failed)
        {
            MessageBox.Show(errMsg + " Character Creation Failed.");
            createView.CharacterCreationDone();
            return;
        }

        selectView.UpdateCharacters(characterCap);
        createView.CharacterCreationDone();
        OnCharacterEntrySelected(0);
        charaSelectPanel.SetActive(true);
        charaCreatePanel.SetActive(false);
    }
    #endregion
}
