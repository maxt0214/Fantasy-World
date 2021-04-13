using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using SkillBridge.Message;

public class UICharacterSelectionView : MonoBehaviour {
	[Header("Character Entries")]
	public Transform characterEntryPrefab;
	public Transform charactersPort;
	private List<UIUserCharacterEntry> characterEntries = new List<UIUserCharacterEntry>();
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
			HighlightSelectedEntry();
		}
	}
	public UIUserCharacterEntry currEntry { get { return characterEntries[_currSelected]; } }

	[Header("Character Avatar Sprites")]
	public Sprite empty;
	public Sprite existed;

	public void UpdateCharacters(int entryCap)
    {
		//Recieve this user's characters from remote and populate our list
		charaInfos = User.Instance.Info.Player.Characters;

		for (int i = 0; i < charaInfos.Count; i++)//Copy all charainfos from remote into our entries
		{ 
			if(i >= characterEntries.Count) //new characters found. Instantiate new ones
            {
				PopulateCharacterEntry(charaInfos[i],existed,i);
			} else //Already existed, update
            {
				characterEntries[i].UpdateInfo(charaInfos[i], existed);
			}
		}
		//Check if we reached the maximum number of characters
		if (characterEntries.Count < entryCap)
        {
			var emptyChara = new NCharacterInfo();
			emptyChara.Class = CharacterClass.None;
			PopulateCharacterEntry(emptyChara, empty, charaInfos.Count);
		}
	}

	/// <summary>
	/// This is only for local debug. Will set all character entries "empty" to be created.
	/// </summary>
	public void UpdateCharactersLocal()
    {
		var emptyChara = new NCharacterInfo();
		emptyChara.Class = CharacterClass.None;
		for (int i = 0; i < 5; i++)
        {
			PopulateCharacterEntry(emptyChara, empty, i);
		}
	}

	private void PopulateCharacterEntry(NCharacterInfo info, Sprite avatar, int index)
    {
		var clone = Instantiate(characterEntryPrefab, charactersPort);
		var charaEntry = clone.GetComponent<UIUserCharacterEntry>();

		if (!charaEntry)
			Debug.LogError("Character Entry does not have the component UIUserCharacterEntry!");

		charaEntry.Init(info,avatar,index);
		characterEntries.Add(charaEntry);
	}

	private void HighlightSelectedEntry()
    {
		if (currEntry.currClass == CharacterClass.None)
			return;

		for (int i = 0; i < characterEntries.Count; i++)
        {
			characterEntries[i].HightLightAvatar(currSelected == i);
		}
	}
}
