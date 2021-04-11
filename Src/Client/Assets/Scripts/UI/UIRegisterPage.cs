using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Services;
using CustomUtilities;

public class UIRegisterPage : MonoBehaviour {

	public InputField userName;
	public InputField password;
	public InputField confirmPassword;

	public Button loginButton;
	public Button backButton;

	void Start()
	{
		UserService.Instance.OnRegister += OnRegister;
	}

	//Will take care of register
	public void OnClickRegister()
	{
		//Validate user name and password
		if (!UserNameValidated(userName.text))
			return;

		if(!PasswordValidated(password.text))
			return;

		//Send User registeration to server
		UserService.Instance.SendRegister(userName.text, password.text);
	}

	public void OnRegister(SkillBridge.Message.Result res, string errMsg)
    {
		Debug.LogFormat("Registeration State: {0}, Error: {1}", res.ToString(), errMsg);
		
		if(res == SkillBridge.Message.Result.Failed)
        {
			MessageBox.Show(errMsg + " Registration Failed.");
			return;
        }

		//TODO: Log User In
    }

	bool UserNameValidated(string text)
    {
		if (string.IsNullOrEmpty(text))
		{
			MessageBox.Show("Please Enter A User Name");
			return false;
		}

		if (text.Contains(" "))
		{
			MessageBox.Show("User Name Cannot Conatin \'Space\'");
			return false;
		}

		return true;
	}

	bool PasswordValidated(string text)
    {
		if (string.IsNullOrEmpty(text))
		{
			MessageBox.Show("Please Enter A Password");
			return false;
		}

		if (text.Contains(" "))
		{
			MessageBox.Show("Password Cannot Conatin \'Space\'");
			return false;
		}
		
		if (!StringUtil.ContainsLower(text))
		{
			MessageBox.Show("Password Must Contains At Least A Letter");
			return false;
		}

		if (!StringUtil.ContainsNumber(text))
		{
			MessageBox.Show("Password Must Contains At Least A Number");
			return false;
		}

		if (text.Length < 8)
		{
			MessageBox.Show("Password Must Contains At Least 8 Characters");
			return false;
		}

		if (text != confirmPassword.text)
		{
			MessageBox.Show("Confirm Password Does Not Match Your Password");
			return false;
		}

		return true;
	}
}
