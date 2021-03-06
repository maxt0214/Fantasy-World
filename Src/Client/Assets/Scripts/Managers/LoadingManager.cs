using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Services;
using Managers;

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

		yield return DataManager.Instance.LoadData();

		UserService.Instance.Init();
		MapService.Instance.Init();
		StatusService.Instance.Init();
		FriendService.Instance.Init();
		TeamService.Instance.Init();
		GuildService.Instance.Init();
		ChatService.Instance.Init();
		BattleService.Instance.Init();
		ArenaService.Instance.Init();
		StoryService.Instance.Init();

		ShopManager.Instance.Init();

		SoundManager.Instance.PlayMusic(SoundDefine.Music_Login);

		//TODO: resource load and percentage update

		//Now loading page fades away and login page shows
		loadingAnim.SetTrigger("LoadingFade");
		yield return new WaitForSeconds(loadingWaitTime);
		loadingPage.SetActive(false);
		loginPage.SetActive(true);
		registerPage.SetActive(false);
	}
}
