using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICharacterView : MonoBehaviour {
	public GameObject[] characters;

	private int _currChar;
	public int currChar
    {
		get
        {
            return _currChar;
        }
        set
        {
            _currChar = value;
            UpdateCharacters();
        }
    }

    private void UpdateCharacters()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == _currChar-1);
        }
    }
}
