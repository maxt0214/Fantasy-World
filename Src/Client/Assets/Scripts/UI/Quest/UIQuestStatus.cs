using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestStatus : MonoBehaviour
{
    public Image[] statusImgs;
    private NPCQuestStatus questStat;

    public void SetQuestStat(NPCQuestStatus stat)
    {
        questStat = stat;
        for(int i = 0; i < statusImgs.Length; i++)
        {
            if(statusImgs[i] != null)
            {
                statusImgs[i].gameObject.SetActive(i == (int)questStat);
            }
        }
    }
}
