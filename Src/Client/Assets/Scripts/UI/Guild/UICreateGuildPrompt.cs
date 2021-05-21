using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICreateGuildPrompt : UIWindow
{
    public InputField inputName;
    public InputField inputOverview;

    private void Start()
    {
        GuildService.Instance.OnGuildCreated = OnGuildCreated;
    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildCreated = null;
    }

    public override void OnClickConfirm()
    {
        if(string.IsNullOrEmpty(inputName.text))
        {
            MessageBox.Show("Please Enter A Name For Your Guild", "Error", MessageBoxType.Error);
            return;
        }

        if (inputName.text.Length < 4 || inputName.text.Length > 20)
        {
            MessageBox.Show("Guild Name Must Be 4-20 characters", "Error", MessageBoxType.Error);
            return;
        }

        if (string.IsNullOrEmpty(inputOverview.text))
        {
            MessageBox.Show("Please Enter A Overview For Your Guild", "Error", MessageBoxType.Error);
            return;
        }

        if (inputOverview.text.Length < 10 || inputOverview.text.Length > 100)
        {
            MessageBox.Show("Guild Overview Must Be 10-100 characters", "Error", MessageBoxType.Error);
            return;
        }
        GuildService.Instance.SendGuildCreation(inputName.text, inputOverview.text);
    }

    private void OnGuildCreated(bool result)
    {
        if (result)
            Close(WindowResult.Confirm);
    }
}
