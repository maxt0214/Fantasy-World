using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TabView : MonoBehaviour
{
    public TabButton[] tabBtns;
    public GameObject[] tabPages;

    public UnityAction<int> OnTabSelected;

    IEnumerator Start()
    {
        for(int i = 0; i < tabBtns.Length; i++)
        {
            tabBtns[i].tabView = this;
            tabBtns[i].tabIndex = i;
        }
        yield return new WaitForEndOfFrame();
        SelectTab(0);
    }

    public void SelectTab(int selectIndx)
    {
        for(int i = 0; i < tabBtns.Length; i++)
        {
            tabBtns[i].Select(i == selectIndx);
            if (i < tabPages.Length)
                tabPages[i].SetActive(i == selectIndx);
        }
        if (OnTabSelected != null)
            OnTabSelected(selectIndx);
    }
}
