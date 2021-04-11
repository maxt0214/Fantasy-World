using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using SkillBridge.Message;

public class UICharacterSelectionView : MonoBehaviour {
	[Header("User Info")]
	public Text playerLevel;
	public Text playerNickName;

	[Header("Character Entries")]
	private List<NCharacterInfo> charaInfos;
	public UIUserCharacterEntry[] characterEntries;

	void Start() {
		if(User.Instance == null)
        {
			var chara = new NCharacterInfo();
			for (int i = 0; i < characterEntries.Length; i++)
				characterEntries[i].Init(chara);
			return;
		}

		//playerLevel.text = User.Instance.Info.Player.Level;
		//playerNickName.text = User.Instance.Info.Player.Name;
		charaInfos = User.Instance.Info.Player.Characters;
		for (int i = 0; i < characterEntries.Length; i++)
        {
			characterEntries[i].Init(charaInfos[i]);
		}
	}

	/// <summary>
	/// Will 
	/// </summary>
	/// <param name="index">Index of character entry</param>
	public void OnCharacterButtonClick(int index)
    {
        switch (characterEntries[index].currClass) 
		{
			case CharacterClass.None:

				break;
			case CharacterClass.Warrior:

				break;
			case CharacterClass.Wizard:

				break;
			case CharacterClass.Archer:

				break;
		}
    }
}
