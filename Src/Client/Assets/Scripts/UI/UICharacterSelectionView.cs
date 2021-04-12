using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using SkillBridge.Message;

public class UICharacterSelectionView : MonoBehaviour {
	public GameObject UICharaCreatePanel;
	public UICharacterView charaView;

	[Header("Character Entries")]
	public UIUserCharacterEntry[] characterEntries;
	private List<NCharacterInfo> charaInfos;

	private int _currSelected;
	public int currSelected
    {
		get
        {
			return _currSelected;
		}
		set
        {
			_currSelected = value;
			OnButtonClick();
		}
    }

	[Header("Character Avatar Sprites")]
	public Sprite empty;
	public Sprite existed;

	void Start() {
		//For local debug
		if(User.Instance.Info == null)
        {
			var chara = new NCharacterInfo();
			chara.Class = CharacterClass.None;
			for (int i = 0; i < characterEntries.Length; i++)
				characterEntries[i].Init(chara, empty);
			return;
		}
		
		charaInfos = User.Instance.Info.Player.Characters;
		for (int i = 0; i < characterEntries.Length; i++)
        {
			if(charaInfos[i].Class == CharacterClass.None)
				characterEntries[i].Init(charaInfos[i], empty);
			else
				characterEntries[i].Init(charaInfos[i], existed);
		}
	}

	private void OnButtonClick()
    {
		if(characterEntries[_currSelected].currClass == CharacterClass.None)
        {
			UICharaCreatePanel.SetActive(true);
			gameObject.SetActive(false);
			return;
		}

		for (int i = 0; i < characterEntries.Length; i++)
        {
			characterEntries[i].HightLightAvatar(currSelected == i);
		}
	}
}
