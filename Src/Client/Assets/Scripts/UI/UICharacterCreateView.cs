using UnityEngine;
using UnityEngine.UI;
using Services;
using System.Linq;

public class UICharacterCreateView : MonoBehaviour {
    public UICharacterView charaView;
    [Header("Character Description")]
    public GameObject[] titles;
    public Text description;

    [Header("Character Classes")]
    public Text[] characterClasses;
    public Text nickName;

    private int selectedClassID;
    private int creatingCharaTID = -1;

    private void Start()
    {
        DataManager.Instance.Load();
        var charaDefs = DataManager.Instance.Characters.Values;
        for (int i = 0; i < characterClasses.Length; i++)
        {
            characterClasses[i].text = charaDefs.Where(c => c.TID == i+1).FirstOrDefault().Name;
        }

        description.text = charaDefs.Where(c => c.TID == 1).FirstOrDefault().Description;

        UserService.Instance.OnCreateCharacter += OnCreateCharacter;
    }

    private void OnDisable()
    {
        UserService.Instance.OnCreateCharacter -= OnCreateCharacter;
    }

    public void OnClickSwitchCharacter(int charaIndex)
    {
        charaView.currChar = charaIndex;
        for(int i = 0; i < titles.Length; i++)
        {
            titles[i].SetActive(i == charaIndex);
        }

        selectedClassID = charaIndex + 1;
        description.text = DataManager.Instance.Characters.Values.Where(c => c.TID == selectedClassID).FirstOrDefault().Description;
    }

    public void OnClickStartAdventure()
    {
        if(creatingCharaTID == -1)
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

        creatingCharaTID = selectedClassID;
        var charaClass = DataManager.Instance.Characters.Values.Where(c => c.TID == creatingCharaTID).FirstOrDefault().Class;
        UserService.Instance.SendCharacterCreation(charaClass, nickName.text);
    }

    public void OnCreateCharacter(SkillBridge.Message.Result res, string errMsg)
    {
        if (res == SkillBridge.Message.Result.Failed)
        {
            MessageBox.Show(errMsg + " Character Creation Failed.");
            creatingCharaTID = -1;//Mark creating character as none
            return;
        }

        Debug.Log("Character Creation Succeeded!!");
        //SceneManager.Instance.LoadScene("MainCity");
    }
}
