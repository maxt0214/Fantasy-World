using Common.Utils;
using SkillBridge.Message;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildMemberItem : ListView.ListViewItem
{
    public Text cName;
    public Text level;
    public Text occupation;
    public Text title;
    public Text joinTime;
    public Text netStat;

    public Image background;

    public NGuildMemberInfo info;

    public override void OnSelected(bool ifSelected)
    {
        background.enabled = ifSelected;
    }

    public void SetMemberInfo(NGuildMemberInfo memberInfo)
    {
        var jointime = TimeUtil.GetTime(memberInfo.joinTime);
        var lastTime = TimeUtil.GetTime(memberInfo.lastOnlineTime);

        info = memberInfo;
        cName.text = info.Info.Name;
        level.text = info.Info.Level.ToString();
        occupation.text = info.Info.Class.ToString();
        title.text = info.Title.ToString();
        joinTime.text = string.Format("{0}/{1}/{2}", jointime.Day , jointime.Month, jointime.Year);
        netStat.text = info.Status == 1 ? "Online" : string.Format("{0}/{1}/{2}", lastTime.Day, lastTime.Month, lastTime.Year);
    }
}
