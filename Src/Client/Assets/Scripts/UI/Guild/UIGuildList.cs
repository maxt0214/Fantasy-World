using Services;
using SkillBridge.Message;
using System.Collections.Generic;
using UnityEngine;

public class UIGuildList : UIWindow
{
    public GameObject itemPrefab;

    public ListView guildList;
    public Transform guildRoot;
    public UIGuildInfo guildInfoPanel;

    public UIGuildItem selectedItem;

    private void Start()
    {
        guildList.OnItemSelected += OnGuildSelected;
        guildInfoPanel.Info = null;
        GuildService.Instance.OnGuildListBack += RefreshUI;
        GuildService.Instance.SendGuildListRequest();
    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildListBack -= RefreshUI;
    }

    private void RefreshUI(List<NGuildInfo> guilds)
    {
        ClearList();
        InitList(guilds);
    }

    private void ClearList()
    {
        guildList.Clear();
    }

    private void InitList(List<NGuildInfo> guilds)
    {
        foreach (var guild in guilds)
        {
            var gameObj = Instantiate(itemPrefab,guildRoot);
            var guildItem = gameObj.GetComponent<UIGuildItem>();
            guildItem.SetGuildInfo(guild);
            guildList.AddItem(guildItem);
        }
    }

    private void OnGuildSelected(ListView.ListViewItem guild)
    {
        selectedItem = guild as UIGuildItem;
        guildInfoPanel.Info = selectedItem.info;
    }

    public void OnClickJoinGuild()
    {
        if(selectedItem == null)
        {
            MessageBox.Show("Please Select A Guild To Join", "Error", MessageBoxType.Error);
            return;
        }

        MessageBox.Show(string.Format("Do You Wanna Join The Guild [{0}]?", selectedItem.info.guildName), "Join Guild", MessageBoxType.Confirm).OnYes = () =>
        {
            GuildService.Instance.SendJoinGuildRequest(selectedItem.info.Id);
        };
    }
}
