using Models;
using Services;
using SkillBridge.Message;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UITeamView : UIWindow
{
    public Text teamTitle;
    public UITeamMemberItem[] teamMembers;
    public ListView teamList;

    private void Start()
    {
        if(User.Instance.teamInfo == null)
        {
            gameObject.SetActive(false);
            return;
        }
        foreach(var member in teamMembers)
        {
            teamList.AddItem(member);
        }
    }

    private void OnEnable()
    {
        UpdateTeamUI();
    }

    public void ShowTeam(bool ifShow)
    {
        gameObject.SetActive(ifShow);
        if (ifShow) UpdateTeamUI();
    }

    private void UpdateTeamUI()
    {
        if (User.Instance.teamInfo == null) return;
        teamTitle.text = string.Format("My Team({0}/5)", User.Instance.teamInfo.Members.Count);

        for(int i = 0; i < 5; i++)
        {
            if (i < User.Instance.teamInfo.Members.Count)
            {
                teamMembers[i].SetMemberInfo(i, User.Instance.teamInfo.Members[i], User.Instance.teamInfo.Members[i].Id == User.Instance.teamInfo.Leader);
                teamMembers[i].gameObject.SetActive(true);
            }
            else
                teamMembers[i].gameObject.SetActive(false);
        }
    }

    public void OnClickLeave()
    {
        MessageBox.Show("Are You Sure You Wanna Leave The Team?", "Leave Team", MessageBoxType.Confirm, "Confirm", "Cancel").OnYes = () =>
        {
            TeamService.Instance.SendLeaveTeamRequest();
        };
    }
}
