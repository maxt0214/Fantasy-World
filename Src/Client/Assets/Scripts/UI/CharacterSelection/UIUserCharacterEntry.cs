using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class UIUserCharacterEntry : MonoBehaviour {
	[Header("Entry Element")]
	public Text characterLevel;
	public Text characterNickName;
	public Image hightlightBG;
	public Image characterAvatar;
	public Button btn;

	public CharacterClass currClass { get; private set; }
	public int? ID;

    public void Init(NCharacterInfo charaInfo, Sprite avatarSprite, int index)
    {
		characterLevel.text = "LEVEL " + charaInfo.Level.ToString();
		characterNickName.text = string.IsNullOrEmpty(charaInfo.Name) ? "New Character" : charaInfo.Name;
		currClass = charaInfo.Class;

		characterAvatar.overrideSprite = avatarSprite;

		if (charaInfo.Class == CharacterClass.None)
			ID = null;
		else
			ID = charaInfo.Id;

		btn.onClick.AddListener(() => { UIUserCharacterView.Instance.OnCharacterEntrySelected(index); });
	}

    public void UpdateInfo(NCharacterInfo charaInfo, Sprite avatarSprite)
	{
		characterLevel.text = "LEVEL " + charaInfo.Level.ToString();
		characterNickName.text = string.IsNullOrEmpty(charaInfo.Name) ? "New Character" : charaInfo.Name;
		currClass = charaInfo.Class;

		characterAvatar.overrideSprite = avatarSprite;

		if (charaInfo.Class == CharacterClass.None)
			ID = null;
		else
			ID = charaInfo.Id;
	}

	public void HightLightAvatar(bool ifHighlighted)
    {
		var color = hightlightBG.color;
		color.a = ifHighlighted ? 1 : 0;
		hightlightBG.color = color;
	}
}
