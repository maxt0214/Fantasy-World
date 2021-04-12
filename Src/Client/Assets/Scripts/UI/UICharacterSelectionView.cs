using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using SkillBridge.Message;

public class UICharacterSelectionView : MonoBehaviour {
	public GameObject UICharaCreatePanel;
	public UICharacterView charaView;
	[Header("User Info")]
	public Text playerLevel;
	public Text playerNickName;

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

	[Header("Button Sprites")]
	public Sprite empty;
	public Sprite warriorAct;
	public Sprite warriorInact;
	public Sprite wizardAct;
	public Sprite wizardInact;
	public Sprite archerAct;
	public Sprite archerInact;

	void Start() {
		if(User.Instance.Info == null)
        {
			var chara = new NCharacterInfo();
			chara.Class = CharacterClass.Archer;
			for (int i = 0; i < characterEntries.Length; i++)
				characterEntries[i].Init(chara, archerAct, archerInact);
			return;
		}

		//playerLevel.text = User.Instance.Info.Player.Level;
		//playerNickName.text = User.Instance.Info.Player.Name;
		charaInfos = User.Instance.Info.Player.Characters;
		for (int i = 0; i < characterEntries.Length; i++)
        {
			Sprite act = null, inact = null;
			switch(charaInfos[i].Class)
            {
				case CharacterClass.None:
					act = empty;
					break;
				case CharacterClass.Warrior:
					act = warriorAct;
					inact = warriorInact;
					break;
				case CharacterClass.Wizard:
					act = wizardAct;
					inact = wizardInact;
					break;
				case CharacterClass.Archer:
					act = archerAct;
					inact = archerInact;
					break;
			}
			characterEntries[i].Init(charaInfos[i], inact, act);
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
			characterEntries[i].SetButtonActive(_currSelected == i);
		}
	}
}
