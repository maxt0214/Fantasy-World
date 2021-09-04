using Managers;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDungeonView : UIWindow
{
    public GameObject itemPrefab;

    public ListView dungeonList;

    public UIDungeonDetailPanel UIDungeonInfo;

    private void Start()
    {
        dungeonList.OnItemSelected += OnDungeonSelected;
    }

    private void OnDestroy()
    {
        dungeonList.OnItemSelected -= OnDungeonSelected;
    }

    public void RefreshUI()
    {
        ClearDungeonList();
        InitDungeonList();
    }

    private void ClearDungeonList()
    {
        dungeonList.Clear();
    }

    private void InitDungeonList()
    {
        foreach (var story in DataManager.Instance.Stories.Values)
        {
            var quest = QuestManager.Instance.GetQuest(story.PreQuest);
            if (quest == null || quest.Info.Status != SkillBridge.Message.QuestStatus.Finished) //Prerequisit Not Satisfied
                continue;

            var storyObj = Instantiate(itemPrefab,dungeonList.transform);
            var dungeonItem = storyObj.GetComponent<UIDungeonItem>();
            dungeonItem.SetDungeonInfo(story,QuestManager.Instance.GetQuest(story.Quest));
            dungeonList.AddItem(dungeonItem);
        }
    }

    private void OnDungeonSelected(ListView.ListViewItem item)
    {
        UIDungeonItem dungeonItem = item as UIDungeonItem;
        UIDungeonInfo.SetDungeonInfo(dungeonItem.Def,dungeonItem.Quest);
    }

    public void OnClickEnterDungeon()
    {
        if(UIDungeonInfo.Story == null)
        {
            MessageBox.Show("Please Select A Dungeon!", "Error", MessageBoxType.Error);
            return;
        }
        if(!StoryManager.Instance.StartStory(UIDungeonInfo.Story.ID))
        {
            MessageBox.Show("Dungeon Does Not Exist!", "Error", MessageBoxType.Error);
            return;
        }
    }
}
