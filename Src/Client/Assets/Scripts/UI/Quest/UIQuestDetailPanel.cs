using Models;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestDetailPanel : MonoBehaviour
{
    public Text questTitle;
    public Text objective;
    public Text overview;

    public UIIconItem[] rewardItems;

    public Text expReward;
    public Text goldReward;

    public void SetQuestInfo(Quest quest)
    {
        questTitle.text = string.Format("[{0}] {1}\n", quest.Define.Type ,quest.Define.Name); 
        if(quest.Info == null) //Available quests
        {
            objective.text = quest.Define.Overview + '\n';
            overview.text = quest.Define.Dialog + '\n';
        } else //Accepted Quests
        {
            if(quest.Info.Status == QuestStatus.Complete || quest.Info.Status == QuestStatus.InProgress)
            {
                objective.text = quest.Define.Overview + '\n';
                overview.text = quest.Define.DialogFinish + '\n';
            }
        }

        expReward.text = "Exp: " + quest.Define.RewardExp.ToString() + '\n';
        goldReward.text = "Gold: " + quest.Define.RewardGold.ToString() + '\n';
    }
}
