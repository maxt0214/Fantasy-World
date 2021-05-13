using Managers;
using System;
using UnityEngine;
using Models;
using Services;

public class UIFriendView : UIWindow
{
    public GameObject itemPrefab;

    public TabView tabs;
    public ListView friendList;
    public Transform listRoot;

    public UIFriendItem selectedFriend;

    private void Start()
    {
        friendList.OnItemSelected += OnFriendSelected;
        tabs.OnTabSelected += OnSelectTab;
        RefreshUI();
        FriendService.Instance.OnFriendUpdate += RefreshUI;
    }

    private void OnDestroy()
    {

    }

    private void OnSelectTab(int tabIndx)
    {
        
    }

    private void RefreshUI()
    {
        ClearFriendList();
        InitFriendList();
    }

    private void ClearFriendList()
    {
        friendList.Clear();
    }

    private void InitFriendList()
    {
        foreach(var friend in FriendManager.Instance.friends)
        {
            var gameObject = Instantiate(itemPrefab, listRoot);
            var friendItem = gameObject.GetComponent<UIFriendItem>();
            friendItem.SetFriendInfo(friend);
            friendList.AddItem(friendItem);
        }
    }

    private void OnFriendSelected(ListView.ListViewItem friend)
    {
        selectedFriend = friend as UIFriendItem;
    }

    public void OnClickAddFriend()
    {
        InputBox.Show("Friend Invitation", "Please Enter A Friend's Name Or ID", "Send", "Cancel", "Come On! Give Me Something So I Can Search").OnSubmit += OnAddFriendSubmit;
    }

    private bool OnAddFriendSubmit(string inputText, out string prompt)
    {
        prompt = "";
        string friendName = "";
        int friendId;
        if (!int.TryParse(inputText, out friendId))
            friendName = inputText;
        if (friendId == User.Instance.CurrentCharacter.Id || friendName == User.Instance.CurrentCharacter.Name)
        {
            prompt = "You Sure You Wanna Add Yourself As Your Friend?";
            return false;
        }

        FriendService.Instance.SendAddFriendRequest(friendId, friendName);
        return true;
    }

    public void OnClickChat()
    {
        MessageBox.Show("Not Yet Available!");
    }

    public void OnClickRemoveFriend()
    {
        if(selectedFriend == null)
        {
            MessageBox.Show("Please Select A Friend To Delet");
            return;
        }
        MessageBox.Show(string.Format("Are You Sure You Wanna Delete Your Friend [{0}]?", selectedFriend.friendInfo.friendInfo.Name), "Delete Friend", MessageBoxType.Confirm, "Delete", "Cancel").OnYes = () =>
        {
            FriendService.Instance.SendRemoveFriendRequest(selectedFriend.friendInfo.Id, selectedFriend.friendInfo.friendInfo.Id);
        };
    }

    public void OnClickInviteFriendToTeam()
    {
        if (selectedFriend == null)
        {
            MessageBox.Show("Please Select A Friend To Invite");
            return;
        }
        if(selectedFriend.friendInfo.Status == 0)
        {
            MessageBox.Show("Please Select A Friend Who Is Online");
            return;
        }
        MessageBox.Show(string.Format("Do You Wanna Invite Your Friend [{0}] To The Team?", selectedFriend.friendInfo.friendInfo.Name), "Team Invitation", MessageBoxType.Confirm, "Invite", "Cancel").OnYes = () =>
        {
            TeamService.Instance.SendInviteFriendToTeamRequest(selectedFriend.friendInfo.friendInfo.Id, selectedFriend.friendInfo.friendInfo.Name);
        };
    }
}
