using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIInputBox : MonoBehaviour 
{
    public Text title;
    public Text message;
    public Text prompts;
    public Button buttonYes;
    public Button buttonNo;
    public InputField input;

    public Text buttonYesTitle;
    public Text buttonNoTitle;

    public delegate bool SubmitHandler(string inputText, out string prompt);
    public event SubmitHandler OnSubmit;
    public UnityAction OnCancel;

    public string emptyPrompt;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Init(string title, string message, string btnOK = "", string btnCancel = "", string emptyPrompt = "")
    {
        if (!string.IsNullOrEmpty(title)) this.title.text = title;
        this.message.text = message;
        prompts.text = null;
        OnSubmit = null;
        this.emptyPrompt = emptyPrompt;

        if (!string.IsNullOrEmpty(btnOK)) buttonYesTitle.text = btnOK;
        if (!string.IsNullOrEmpty(btnCancel)) buttonNoTitle.text = btnCancel;

        buttonYes.onClick.AddListener(OnClickYes);
        buttonNo.onClick.AddListener(OnClickNo);
    }

    void OnClickYes()
    {
        prompts.text = "";
        if(string.IsNullOrEmpty(input.text))
        {
            prompts.text = emptyPrompt;
            return;
        }
        if(OnSubmit != null)
        {
            string prompt;
            if(!OnSubmit(input.text,out prompt))
            {
                prompts.text = prompt;
                return;
            }
        }
        Destroy(gameObject);
    }

    void OnClickNo()
    {
        Destroy(gameObject);
        if (OnCancel != null)
            OnCancel();
    }
}
