using Common.Data;
using Managers;
using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuild : UIWindow
{
    public GameObject itemPrefab;

    public ListView memberList;
    public Transform memberRoot;
    public UIGuildInfo guildInfoPanel;

    public UIGuildMemberItem selectedItem;

    public GameObject adminPanel;
    public GameObject leaderPanel;
    public GameObject editBtn;

    private void Start()
    {
        GuildService.Instance.OnGuildUpdate += RefreshUI;
        memberList.OnItemSelected += OnMemberSelected;
        RefreshUI();
    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate -= RefreshUI;
    }

    private void RefreshUI()
    {
        guildInfoPanel.Info = GuildManager.Instance.guildInfo;
        ClearList();
        InitList();

        adminPanel.SetActive(GuildManager.Instance.myMemberInfo.Title > GuildTitle.None);
        var ifleader = GuildManager.Instance.myMemberInfo.Title == GuildTitle.President;
        leaderPanel.SetActive(ifleader);
        editBtn.SetActive(ifleader);
    }

    private void ClearList()
    {
        memberList.Clear();
    }

    private void InitList()
    {
        foreach(var member in GuildManager.Instance.members)
        {
            var gameObj = Instantiate(itemPrefab, memberRoot);
            var memberItem = gameObj.GetComponent<UIGuildMemberItem>();
            memberItem.SetMemberInfo(member);
            memberList.AddItem(memberItem);
        }
    }

    private void OnMemberSelected(ListView.ListViewItem member)
    {
        selectedItem = member as UIGuildMemberItem;
    }

    public void OnClickTransferLeader()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("Please Select A Member To Transfer Your Title To", "Error");
            return;
        }
        if (selectedItem.info.Title == GuildTitle.President)
        {
            MessageBox.Show("HaHa, Funny. Transferring Your Power To Yourself?");
            return;
        }
        MessageBox.Show(string.Format("Do You Wanna Transfer Your Position To [{0}]?", selectedItem.info.Info.Name), "Transfer Leader", MessageBoxType.Confirm, "Confirm", "Cancel").OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminAction.Transfer, selectedItem.info.Info.Id);
        };
    }

    public void OnClickPromote()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("Please Select A Member To Promote");
            return;
        }
        if (selectedItem.info.Title != GuildTitle.None)
        {
            MessageBox.Show("The Member Is Already Promoted");
            return;
        }
        MessageBox.Show(string.Format("Do You Wanna Promote [{0}]?", selectedItem.info.Info.Name), "Promote", MessageBoxType.Confirm, "Confirm", "Cancel").OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminAction.Promote, selectedItem.info.Info.Id);
        };
    }

    public void OnClickDemote()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("Please Select A Member To Demote");
            return;
        }
        if (selectedItem.info.Title == GuildTitle.President)
        {
            MessageBox.Show("Hi! You Don't Be Disrespectful To The Leader");
            return;
        }
        if (selectedItem.info.Title == GuildTitle.None)
        {
            MessageBox.Show("Your Cant Demote A Mere Member");
            return;
        }
        MessageBox.Show(string.Format("Do You Wanna Demote [{0}]?", selectedItem.info.Info.Name), "Demote", MessageBoxType.Confirm, "Confirm", "Cancel").OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminAction.Demote, selectedItem.info.Info.Id);
        };
    }

    public void OnClickApplicantList()
    {
        UIManager.Instance.Show<UIGuildApplicantList>();
    }

    public void OnClickRemove()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("Please Select A Member To Remove");
            return;
        }
        MessageBox.Show(string.Format("Do You Wanna Kick [{0}] Out Of The Guild?", selectedItem.info.Info.Name), "Kick Out", MessageBoxType.Confirm, "Confirm", "Cancel").OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminAction.Kickout, selectedItem.info.Info.Id);
        };
    }

    public void OnClickChat()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("Please Select A Member To Chat With");
            return;
        }
    }

    public void OnClickLeave()
    {
        MessageBox.Show(string.Format("Do You Wanna Leave The Guild?", selectedItem.info.Info.Name), "Leave Guild", MessageBoxType.Confirm, "Confirm", "Cancel").OnYes = () =>
        {
            GuildService.Instance.SendLeaveGuild();
        };
    }
}
