using Common.Data;
using Models;
using SkillBridge.Message;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildInfo : MonoBehaviour
{
    public Text guildName;
    public Text guildID;
    public Text leader;
    public Text guildOverview;
    public Text memberCount;

    private NGuildInfo info;
    public NGuildInfo Info
    {
        get { return info; }
        set { info = value; UpdateUI(); }
    }

    private void UpdateUI()
    {
        if (info == null)
        {
            guildName.text = "None";
            guildID.text = "ID: 0";
            leader.text = "None";
            guildOverview.text = "";
            memberCount.text = string.Format("Member: 0/{0}", 40);
        } else
        {
            guildName.text = info.guildName;
            guildID.text = "ID: " + info.Id.ToString();
            leader.text = "Leader: " + info.leaderName;
            guildOverview.text = info.Overview;
            memberCount.text = string.Format("Member: {0}/{1}", info.memberCount, 40);
        }
    }
}
