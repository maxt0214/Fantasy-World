using Common.Battle;
using Common.Data;
using Managers;
using Models;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkillView : UIWindow
{
    public GameObject itemPrefab;

    public ListView memberList;
    public Transform memberRoot;
    public UISkillInfo skillInfo;

    private UISkillItem selectedItem;
    public UISkillItem SelectedItem
    {
        get { return selectedItem; }
        set
        {
            selectedItem = value;
            if(selectedItem != null) skillInfo.SetSkillInfo(selectedItem.skill);
        }
    }

    private void Start()
    {
        RefreshUI();
        memberList.OnItemSelected += OnMemberSelected;
    }

    private void OnDestroy()
    {

    }

    private void RefreshUI()
    {
        ClearEquipList();
        InitEquipList();
    }

    private void ClearEquipList()
    {
        memberList.Clear();
    }

    private void InitEquipList()
    {
        var skills = User.Instance.currentCharacter.skillMgr.skills;
        foreach (var skill in skills)
        {
            var gameObj = Instantiate(itemPrefab, memberRoot);
            var memberItem = gameObj.GetComponent<UISkillItem>();
            memberItem.SetSkillInfo(skill);
            memberList.AddItem(memberItem);
        }
    }

    private void OnMemberSelected(ListView.ListViewItem member)
    {
        SelectedItem = member as UISkillItem;
    }

    public void OnClickUpgradeSkill()
    {

    }
}
