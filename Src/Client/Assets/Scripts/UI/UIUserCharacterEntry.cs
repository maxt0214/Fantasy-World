using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using SkillBridge.Message;

public class UIUserCharacterEntry : MonoBehaviour {
	[Header("Character Image")]
	public Material Empty;
	public Material Warrior;
	public Material Wizard;
	public Material Archer;

	[Header("Entry Element")]
	public Text characterLevel;
	public Text characterNickName;
	public Image selectButtonBG;

	public CharacterClass currClass { get; private set; }
	
	/// <summary>
	/// Initate character entry.
	/// </summary>
	public void Init(NCharacterInfo charaInfo)
	{
		characterLevel.text = "LEVEL " + charaInfo.Level.ToString();
		characterNickName.text = string.IsNullOrEmpty(charaInfo.Name) ? "New Character" : charaInfo.Name;
		currClass = charaInfo.Class;

		switch (currClass)
        {
			case CharacterClass.None:
				selectButtonBG.material = Empty;
				break;
			case CharacterClass.Warrior:
				selectButtonBG.material = Warrior;
				break;
			case CharacterClass.Wizard:
				selectButtonBG.material = Wizard;
				break;
			case CharacterClass.Archer:
				selectButtonBG.material = Archer;
				break;
		}
	}
}
