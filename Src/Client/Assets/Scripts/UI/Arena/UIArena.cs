using Managers;
using Models;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIArena : MonoSingleton<UIArena>
{
    public Text roundTxt;
    public Text countDownTxt;
    public Animator anim;

    private int myScore;
    private int otherScore;

    protected override void OnStart()
    {
        roundTxt.enabled = false;
        countDownTxt.enabled = false;
        ArenaManager.Instance.SendReady();
    }

    private void Update()
    {

    }

    public void ShowCountDown()
    {
        StartCoroutine(CountDown(10));
    }

    private IEnumerator CountDown(int count)
    {
        int waitTime = count;
        roundTxt.text = "Round " + ArenaManager.Instance.Round.ToString();
        roundTxt.enabled = true;
        countDownTxt.enabled = true;
        while(waitTime > 0)
        {
            SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_COUNTDOWN);
            countDownTxt.text = waitTime.ToString();
            yield return new WaitForSeconds(1f);
            waitTime--;
        }

        countDownTxt.text = "Ready";
    }

    public void ShowRoundStart(int round, ArenaInfo arenaInfo)
    {
        countDownTxt.text = "Off You Go!";
        anim.SetTrigger("Fade");
    }

    public void ShowRoundResult(int round, ArenaInfo arenaInfo)
    {
        countDownTxt.enabled = true;

        if (arenaInfo.Blue.Cid == User.Instance.currentCharacter.Id)
        {
            myScore = arenaInfo.Blue.Score;
            otherScore = arenaInfo.Red.Score;
        }
        else
        {
            myScore = arenaInfo.Red.Score;
            otherScore = arenaInfo.Blue.Score;
        }
        
        if (round < 3)
        {
            if (myScore > otherScore)
                countDownTxt.text = string.Format("{0} : {1}\nYou Are Wining!", myScore, otherScore);
            else if(myScore < otherScore)
                countDownTxt.text = string.Format("{0} : {1}\nYou Are Losing!", myScore, otherScore);
            else
                countDownTxt.text = string.Format("{0} : {1}\nBreak The Tie!", myScore, otherScore);
        } 
        else
        {
            if(arenaInfo.Red == null || arenaInfo.Blue == null)
                countDownTxt.text = "The Other Player Left. You Won!";
            else
            {
                if (myScore > otherScore)
                    countDownTxt.text = string.Format("{0} : {1}\nYou Won!", myScore, otherScore);
                else if (myScore < otherScore)
                    countDownTxt.text = string.Format("{0} : {1}\nYou Lost!", myScore, otherScore);
                else
                    countDownTxt.text = string.Format("{0} : {1}\nA Tie!", myScore, otherScore);
            }
        }

        anim.SetTrigger("Fade");
    }
}
