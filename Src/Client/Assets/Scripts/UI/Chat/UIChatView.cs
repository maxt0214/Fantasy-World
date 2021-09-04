using Candlelight.UI;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChatView : UIWindow
{
    public HyperText chatContent;
    public TabView channelTabs;

    public InputField chatInput;
    public Text chatTarget;

    public Dropdown channelSelection;

    private int inputId = -1;

    private void Start()
    {
        channelTabs.OnTabSelected += OnChannelDisplayedSelected;
        ChatManager.Instance.OnChat += RefreshUI;
        inputId = InputManager.Instance.RegisterInputSource();
    }

    private void Update()
    {
        if(inputId != -1)
        {
            InputManager.Instance.EnableInputSource(inputId, chatInput.isFocused);
        }
    }

    private void OnDestroy()
    {
        ChatManager.Instance.OnChat -= RefreshUI;
    }

    private void OnChannelDisplayedSelected(int channelIndx)
    {
        ChatManager.Instance.displayChannel = (ChatManager.LocalChannel)channelIndx;
        RefreshUI();
    }

    private void RefreshUI()
    {
        chatContent.text = ChatManager.Instance.GetCurrentMessage();
        channelSelection.value = (int)ChatManager.Instance.sendChannel - 1;
        if(ChatManager.Instance.sendChannel == ChatManager.LocalChannel.Private)
        {
            chatTarget.gameObject.SetActive(true);
            if (ChatManager.Instance.chatTarget != 0)
                chatTarget.text = ChatManager.Instance.chatName;
            else
                chatTarget.text = "N/A";
        } else
        {
            chatTarget.gameObject.SetActive(false);
        }
    }

    public void OnClickChatLink(HyperText text, HyperText.LinkInfo link)
    {
        if (string.IsNullOrEmpty(link.Name)) return;
        //<a name="c:1:Name" class="player">Name</a>
        if(link.Name.StartsWith("c:"))
        {
            string[] strs = link.Name.Split(":".ToCharArray());
            UIChatPopUp menu = UIManager.Instance.Show<UIChatPopUp>();
            menu.targetId = int.Parse(strs[1]);
            menu.targetName = strs[2];
        }
    }

    public void OnClickSend()
    {
        OnEndInput(chatInput.text);
    }

    public void OnEndInput(string text)
    {
        if (!string.IsNullOrEmpty(text)) SendChatMessage(text);
        chatInput.text = "";
    }

    private void SendChatMessage(string msg)
    {
        ChatManager.Instance.SendChatMessage(msg);
    }

    public void OnClickSendChannelChanged(int indx)
    {
        if (ChatManager.Instance.sendChannel == (ChatManager.LocalChannel)(indx + 1)) return;

        if(!ChatManager.Instance.SetSendChannel((ChatManager.LocalChannel)(indx + 1)))
        {
            channelSelection.value = (int)ChatManager.Instance.sendChannel - 1;
        } else
        {
            RefreshUI();
        }
    }
}
