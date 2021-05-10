using Common.Data;
using Models;
using SkillBridge.Message;
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

        for(int i = 0; i < rewardItems.Length; i++)
        {
            rewardItems[i].ResetIcon();
        }

        ItemDefine rewardItem;
        if (DataManager.Instance.Items.TryGetValue(quest.Define.RewardItem1, out rewardItem)) {
            rewardItems[0].SetIcon(rewardItem.Icon, quest.Define.RewardItem1Count.ToString());
            if (DataManager.Instance.Items.TryGetValue(quest.Define.RewardItem2, out rewardItem))
            {
                rewardItems[1].SetIcon(rewardItem.Icon, quest.Define.RewardItem2Count.ToString());
                if (DataManager.Instance.Items.TryGetValue(quest.Define.RewardItem3, out rewardItem))
                    rewardItems[2].SetIcon(rewardItem.Icon, quest.Define.RewardItem3Count.ToString());
            }
        }

        expReward.text = "Exp: " + quest.Define.RewardExp.ToString() + '\n';
        goldReward.text = "Gold: " + quest.Define.RewardGold.ToString() + '\n';
    }
}
