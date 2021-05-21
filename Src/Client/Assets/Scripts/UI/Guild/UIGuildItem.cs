using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildItem : ListView.ListViewItem
{
    public Text id;
    public Text gName;
    public Text memberCount;
    public Text leader;

    public Image background;

    public NGuildInfo info;

    public override void OnSelected(bool ifSelected)
    {
        background.enabled = ifSelected;
    }

    public void SetGuildInfo(NGuildInfo guildInfo)
    {
        info = guildInfo;
        id.text = info.Id.ToString();
        gName.text = info.guildName;
        memberCount.text = info.memberCount.ToString();
        leader.text = info.leaderName;
    }
}
