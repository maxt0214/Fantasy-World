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

	void Start () {
		UserService.Instance.OnLogin += OnLogin;
	}
	
	void Update () {
		
	}

	//Will take care of user log in
	public void OnClickLogin()
    {
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
		Debug.LogFormat("Login State: {0}, Error: {1}", res.ToString(), errMsg);

		if (res == SkillBridge.Message.Result.Failed)
		{
			MessageBox.Show(errMsg + " Login Failed.");
			return;
		}

		//TODO: Log User In
	}
}
