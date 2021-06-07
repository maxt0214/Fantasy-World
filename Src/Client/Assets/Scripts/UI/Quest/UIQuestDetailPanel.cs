using Common.Data;
using Models;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;
using Managers;

public class UIQuestDetailPanel : MonoBehaviour
{
    public Text questTitle;
    public Text objective;
    public Text overview;

    public UIIconItem[] rewardItems;

    public Text expReward;
    public Text goldReward;

    public Button navBtn;
    private int npc = 0;

    public Button abandonBtn;

    public void SetQuestInfo(Quest quest)
    {
        questTitle.text = string.Format("[{0}] {1}", quest.Define.Type ,quest.Define.Name); 
        if(quest.Info == null) //Available quests
        {
            objective.text = quest.Define.Overview;
            overview.text = quest.Define.Dialog;
        } else //Accepted Quests
        {
            if(quest.Info.Status == QuestStatus.Complete || quest.Info.Status == QuestStatus.InProgress)
            {
                objective.text = quest.Define.Overview;
                overview.text = quest.Define.DialogFinish;
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

        expReward.text = "Exp: " + quest.Define.RewardExp.ToString();
        goldReward.text = "Gold: " + quest.Define.RewardGold.ToString();

        if(quest.Info == null)
        {
            npc = quest.Define.AcceptNPC;
        } else if(quest.Info.Status == QuestStatus.Complete)
        {
            npc = quest.Define.SubmitNPC;
        }
        if(navBtn != null) navBtn.gameObject.SetActive(npc > 0);
        if(abandonBtn != null) abandonBtn.gameObject.SetActive(quest.Info != null && quest.Info.Status == QuestStatus.InProgress);

        foreach (var fitter in GetComponentsInChildren<ContentSizeFitter>())
        {
            fitter.SetLayoutVertical();
        }
    }

    public void OnClickAbandonQuest()
    {

    }

    public void OnClickTrackQuest()
    {
        var target = NPCManager.Instance.GetNPCPosition(npc);
        User.Instance.currentCharacterObj.StartNav(target);
        UIManager.Instance.Close<UIQuestSystem>();
    }
}
