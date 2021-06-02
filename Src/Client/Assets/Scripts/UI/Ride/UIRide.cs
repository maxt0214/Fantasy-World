using Common.Data;
using Managers;
using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRide : UIWindow
{
    public GameObject itemPrefab;

    public ListView memberList;
    public Transform memberRoot;

    public UIRideInfo rideInfoPanel;

    public UIRideItem selectedItem;

    private void Start()
    {
        memberList.OnItemSelected += OnMemberSelected;
        RefreshUI();
        RideManager.Instance.RideChanged += RefreshUI;
    }

    private void OnDestroy()
    {
        RideManager.Instance.RideChanged -= RefreshUI;
    }

    private void RefreshUI()
    {
        ClearList();
        InitList();
    }

    private void ClearList()
    {
        memberList.Clear();
    }

    private void InitList()
    {
        foreach (var ride in RideManager.Instance.Rides)
        {
            if(ride.itemDef.Class == CharacterClass.None || ride.itemDef.Class == User.Instance.CurrentCharacter.Class)
            {
                var gameObj = Instantiate(itemPrefab, memberRoot);
                var rideItem = gameObj.GetComponent<UIRideItem>();
                rideItem.SetRideItemInfo(ride,this,false);
                memberList.AddItem(rideItem);
            }
        }
    }

    private void OnMemberSelected(ListView.ListViewItem member)
    {
        selectedItem = member as UIRideItem;
        rideInfoPanel.SetRideInfo(selectedItem.ride.itemDef.Name, selectedItem.ride.itemDef.Description);
    }

    public void OnClickSummonMount()
    {
        if(selectedItem == null)
        {
            MessageBox.Show("Please Select A Mount To Ride On", "Hint");
        }
        User.Instance.Ride(selectedItem.ride.Id);
    }
}
