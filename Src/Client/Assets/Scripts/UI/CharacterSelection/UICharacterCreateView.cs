using UnityEngine;
using UnityEngine.UI;
using Services;
using System.Linq;
using SkillBridge.Message;

public class UICharacterCreateView : MonoBehaviour {
    public UICharacterView charaView;
    [Header("Character Description")]
    public GameObject[] titles;
    public Text description;

    [Header("Character Classes")]
    public Text[] characterClasses;
    public Text nickName;

    private int selectedCharaClass;
    private bool creatingCharacter = false;

    public void Init()
    {
        DataManager.Instance.Load();
        for (int i = 0; i < characterClasses.Length; i++)
        {
            characterClasses[i].text = ((CharacterClass)i+1).ToString();
        }

        description.text = DataManager.Instance.Characters.Values.Where(c => c.Class == (CharacterClass)1).FirstOrDefault().Description;
    }

    public void SwitchCharacter(int charaIndex, CharacterClass currClass)
    {
        selectedCharaClass = (int)currClass;
        for (int i = 0; i < titles.Length; i++)
        {
            titles[i].SetActive(i == charaIndex);
        }

        description.text = DataManager.Instance.Characters.Values.Where(c => c.Class == currClass).FirstOrDefault().Description;
    }

    public void CreateCharacter()
    {
        if(selectedCharaClass == 0)
        {
            MessageBox.Show("You must select a character class to create your character!");
            return;
        }

        if(creatingCharacter)
        {
            MessageBox.Show("Already Trying To Create A Character. Please Be Patient!");
            return;
        }

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

        if (nickName.text.Length > 10)
        {
            MessageBox.Show("Your Nick Name Must Be 0~10 characters");
            return;
        }

        creatingCharacter = true;
        UserService.Instance.SendCharacterCreation((CharacterClass)selectedCharaClass, nickName.text);
    }

    public void CharacterCreationDone()
    {
        creatingCharacter = false;
    }
}
