using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class UIUserCharacterEntry : MonoBehaviour {
	[Header("Entry Element")]
	public Text characterLevel;
	public Text characterNickName;
	public Image selectButtonBG;

	public CharacterClass currClass { get; private set; }

	private Sprite inactiveBtn;
	private Sprite activeBtn;

	/// <summary>
	/// Initate character entry.
	/// </summary>
	public void Init(NCharacterInfo charaInfo, Sprite inactive, Sprite active = null)
	{
		characterLevel.text = "LEVEL " + charaInfo.Level.ToString();
		characterNickName.text = string.IsNullOrEmpty(charaInfo.Name) ? "New Character" : charaInfo.Name;
		currClass = charaInfo.Class;

		inactiveBtn = inactive;
		activeBtn = active;

		selectButtonBG.overrideSprite = inactiveBtn;
	}

	public void SetButtonActive(bool ifActive)
	{
		if (!inactiveBtn || !activeBtn)
			return;

		selectButtonBG.overrideSprite = (ifActive) ? activeBtn : inactiveBtn;
	}
}
