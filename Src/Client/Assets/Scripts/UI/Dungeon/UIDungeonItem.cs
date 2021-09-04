using Common.Data;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDungeonItem : ListView.ListViewItem
{
    public Text dungeonName;

    public Image background;
    public Sprite select;
    public Sprite normal;

    public StoryDefine Def;
    public Quest Quest;

    public override void OnSelected(bool ifSelected)
    {
        background.overrideSprite = ifSelected ? select : normal;
    }

    public void SetDungeonInfo(StoryDefine define, Quest quest)
    {
        Def = define;
        Quest = quest;
        if (dungeonName != null) dungeonName.text = Def.Name;
    }
}
