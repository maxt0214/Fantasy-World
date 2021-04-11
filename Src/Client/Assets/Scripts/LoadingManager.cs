﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour {
	[Header("Windows")]
	public GameObject loadingPage;
	public GameObject loginPage;
	public GameObject registerPage;
	public Animator loadingAnim;

	[Header("Loading Displays")]
	public Slider loadingBar;
	public Text loadingPercentage;
	public Text loadingTxt;
	public float loadingWaitTime;
	
	IEnumerator Start () {
		log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.xml"));
		UnityLogger.Init();
		Common.Log.Init("Unity");
		Common.Log.Info("Loading Manager Awake");

		//Loading page should be displayed first
		loadingPage.SetActive(true);
		loginPage.SetActive(false);
		registerPage.SetActive(false);
		yield return new WaitForSeconds(loadingWaitTime);

		//TODO: resource load and percentage update

		//Now loading page fades away and login page shows
		loadingAnim.SetTrigger("LoadingFade");
		yield return new WaitForSeconds(loadingWaitTime);
		loadingPage.SetActive(false);
		loginPage.SetActive(true);
		registerPage.SetActive(false);
	}
}