using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIMessageBox : MonoBehaviour {

    public Text title;
    public Text message;
    public Image[] icons;
    public Button buttonYes;
    public Button buttonNo;
    public Button buttonClose;

    public Text buttonYesTitle;
    public Text buttonNoTitle;

    public UnityAction OnYes;
    public UnityAction OnNo;
    

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Init(string title, string message, MessageBoxType type = MessageBoxType.Information, string btnOK = "", string btnCancel = "")
    {
        if (!string.IsNullOrEmpty(title)) this.title.text = title;
        this.message.text = message;
        icons[0].enabled = type == MessageBoxType.Information;
        icons[1].enabled = type == MessageBoxType.Confirm;
        icons[2].enabled = type == MessageBoxType.Error;

        if (!string.IsNullOrEmpty(btnOK)) buttonYesTitle.text = btnOK;
        if (!string.IsNullOrEmpty(btnCancel)) buttonNoTitle.text = btnCancel;

        buttonYes.onClick.AddListener(OnClickYes);
        buttonNo.onClick.AddListener(OnClickNo);

        buttonNo.gameObject.SetActive(type == MessageBoxType.Confirm);
    }

    void OnClickYes()
    {
        Destroy(gameObject);
        if (OnYes != null)
            OnYes();
    }

    void OnClickNo()
    {
        Destroy(gameObject);
        if (OnNo != null)
            OnNo();
    }
}
