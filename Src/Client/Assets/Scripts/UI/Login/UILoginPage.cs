using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Services;

public class UILoginPage : MonoBehaviour {

	public InputField userName;
	public InputField password;

	public Button loginButton;
	public Button registerButton;

    private void OnEnable()
    {
		UserService.Instance.OnLogin += OnLogin;
	}

    private void OnDisable()
	{
		UserService.Instance.OnLogin -= OnLogin;
	}

	//Will take care of user log in
	public void OnClickLogin()
    {
		SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
		if (string.IsNullOrEmpty(userName.text))
		{
			MessageBox.Show("Please Enter Your User Name");
			return;
		}

		if (string.IsNullOrEmpty(password.text))
		{
			MessageBox.Show("Please Enter Your Password");
			return;
		}

		UserService.Instance.SendLogin(userName.text, password.text);
	}

	public void OnLogin(SkillBridge.Message.Result res, string errMsg)
	{
		SoundManager.Instance.PlayMusic(SoundDefine.Music_Select);
		if (res == SkillBridge.Message.Result.Failed)
		{
			MessageBox.Show(errMsg + " Login Failed.");
			return;
		}

		Debug.Log("Login Succeed! Tranferring to character creation scene!");
		SceneManager.Instance.LoadScene("CharacterCreation");
	}
}
