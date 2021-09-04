using Common.Data;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDungeonDetailPanel : MonoBehaviour
{
    public Text dungeonTitle;
    public Text overview;

    public UIIconItem[] rewardItems;

    public Text expReward;
    public Text goldReward;

    public StoryDefine Story;
    public Quest Quest;

    public void SetDungeonInfo(StoryDefine story, Quest quest)
    {
        Story = story;
        Quest = quest;

        dungeonTitle.text = Story.Name;
        overview.text = Story.Description;

        for (int i = 0; i < rewardItems.Length; i++)
        {
            rewardItems[i].ResetIcon();
        }

        ItemDefine rewardItem;
        if (DataManager.Instance.Items.TryGetValue(Quest.Define.RewardItem1, out rewardItem))
        {
            rewardItems[0].SetIcon(rewardItem.Icon, Quest.Define.RewardItem1Count.ToString());
            if (DataManager.Instance.Items.TryGetValue(Quest.Define.RewardItem2, out rewardItem))
            {
                rewardItems[1].SetIcon(rewardItem.Icon, Quest.Define.RewardItem2Count.ToString());
                if (DataManager.Instance.Items.TryGetValue(Quest.Define.RewardItem3, out rewardItem))
                    rewardItems[2].SetIcon(rewardItem.Icon, Quest.Define.RewardItem3Count.ToString());
            }
        }

        expReward.text = "Exp: " + Quest.Define.RewardExp.ToString();
        goldReward.text = "Gold: " + Quest.Define.RewardGold.ToString();

        foreach (var fitter in GetComponentsInChildren<ContentSizeFitter>())
        {
            fitter.SetLayoutVertical();
        }
    }
}
