using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIQuestDialog : UIWindow
{
    public UIQuestDetailPanel questInfo;
    public GameObject acceptBtns;
    public GameObject finishBtns;

    public Quest quest;

    public void SetQuest(Quest currQuest)
    {
        quest = currQuest;
        UpdateQuestInfo();
        if(quest.Info == null)
        {
            acceptBtns.SetActive(true);
            finishBtns.SetActive(false);
        } else
        {
            if(quest.Info.Status == SkillBridge.Message.QuestStatus.Complete)
            {
                acceptBtns.SetActive(false);
                finishBtns.SetActive(true);
            } else
            {
                acceptBtns.SetActive(false);
                finishBtns.SetActive(false);
            }
        }
    }

    public void UpdateQuestInfo()
    {
        if(quest != null)
        {
            if (questInfo != null)
                questInfo.SetQuestInfo(quest);
        }
    }
}
