using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class UIFriendItem : ListView.ListViewItem
{
    public Text friendName;
    public Text level;
    public Text occupation;
    public Text netStat;

    public Image background;
    public Sprite select;
    public Sprite normal;

    public NFriendInfo friendInfo;

    public override void OnSelected(bool ifSelected)
    {
        background.overrideSprite = ifSelected ? select : normal;
    }

    public void SetFriendInfo(NFriendInfo friendInfo)
    {
        this.friendInfo = friendInfo;
        friendName.text = this.friendInfo.friendInfo.Name;
        level.text = this.friendInfo.friendInfo.Level.ToString();
        occupation.text = this.friendInfo.friendInfo.Class.ToString();
        netStat.text = this.friendInfo.Status == 1 ? "Online" : "Offline";
    }
}
