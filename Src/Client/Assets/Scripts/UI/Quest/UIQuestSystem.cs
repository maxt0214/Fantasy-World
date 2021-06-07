using Managers;
using System;
using UnityEngine;
using UnityEngine.UI;
using Common.Data;
using Models;

public class UIQuestSystem : UIWindow
{
    public Text Title;

    public GameObject itemPrefab;

    public TabView tabs;
    public ListView mainQuestList;
    public ListView branchQuestList;

    public UIQuestDetailPanel UIQuestInfo;

    private bool showAvailableList = false;

    private void Start()
    {
        mainQuestList.OnItemSelected += OnQuestSelected;
        branchQuestList.OnItemSelected += OnQuestSelected;
        tabs.OnTabSelected += OnSelectTab;
        RefreshUI();
        QuestManager.Instance.onQuestStatusChanged += RefreshUI;
    }

    private void OnDestroy()
    {
        QuestManager.Instance.onQuestStatusChanged -= RefreshUI;
    }

    private void OnSelectTab(int tabIndx)
    {
        showAvailableList = tabIndx == 1;
        RefreshUI();
    }

    private void RefreshUI(Quest quest = null)
    {
        ClearQuestList();
        InitQuestList();
    }

    private void ClearQuestList()
    {
        mainQuestList.Clear();
        branchQuestList.Clear();
    }

    private void InitQuestList()
    {
        foreach(var kv in QuestManager.Instance.allQuests)
        {
            if(showAvailableList) //Available Quests
            {
                if (kv.Value.Info != null) //NQuestInfo exists. This is a quest we already accepted. Skip
                    continue;
            } else //Ongoing Quests
            {
                if (kv.Value.Info == null) //NQuestInfo does not exist. This is a quest we have not yet accepted. Skip
                    continue;
                if (kv.Value.Info.Status == SkillBridge.Message.QuestStatus.Finished)
                    continue;
            }
            var questObj = Instantiate(itemPrefab, kv.Value.Define.Type == QuestType.Main ? mainQuestList.transform : branchQuestList.transform);
            var questItem = questObj.GetComponent<UIQuestItem>();
            questItem.SetQuestInfo(kv.Value);
            if (kv.Value.Define.Type == QuestType.Main)
                mainQuestList.AddItem(questItem);
            else
                branchQuestList.AddItem(questItem);
        }
    }

    private void OnQuestSelected(ListView.ListViewItem quest)
    {
        UIQuestItem questItem = quest as UIQuestItem;
        UIQuestInfo.SetQuestInfo(questItem.quest);

        if (questItem.owner == mainQuestList && branchQuestList.SelectedItem != null)
            branchQuestList.ClearSelection();
        if (questItem.owner == branchQuestList && mainQuestList.SelectedItem != null)
            mainQuestList.ClearSelection();
    }
}
