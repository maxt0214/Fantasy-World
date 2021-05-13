using Models;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestItem : ListView.ListViewItem
{
    public Text questContent;

    public Image background;
    public Sprite select;
    public Sprite normal;

    public Quest quest;

    public override void OnSelected(bool ifSelected)
    {
        background.overrideSprite = ifSelected ? select : normal;
    }

    public void SetQuestInfo(Quest quest)
    {
        this.quest = quest;
        if (questContent!= null) questContent.text = quest.Define.Name;
    }
}
