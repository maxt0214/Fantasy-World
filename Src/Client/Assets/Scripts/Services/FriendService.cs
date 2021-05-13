using Models;
using Network;
using SkillBridge.Message;
using System;
using UnityEngine;
using Managers;
using UnityEngine.Events;

namespace Services
{
    class FriendService : Singleton<FriendService>, IDisposable
    {
        public UnityAction OnFriendUpdate;

        public void Init()
        {

        }

        public FriendService()
        {
            MessageDistributer.Instance.Subscribe<FriendAddRequest>(OnAddFriendRequest);
            MessageDistributer.Instance.Subscribe<FriendAddResponse>(OnAddFriendResponse);
            MessageDistributer.Instance.Subscribe<FriendListResponse>(OnFriendList);
            MessageDistributer.Instance.Subscribe<FriendRemoveResponse>(OnRemoveFriend);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<FriendAddRequest>(OnAddFriendRequest);
            MessageDistributer.Instance.Unsubscribe<FriendAddResponse>(OnAddFriendResponse);
            MessageDistributer.Instance.Unsubscribe<FriendListResponse>(OnFriendList);
            MessageDistributer.Instance.Unsubscribe<FriendRemoveResponse>(OnRemoveFriend);
        }

        public void SendAddFriendRequest(int friendId, string friendName)
        {
            Debug.Log("SendAddFriendRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.friendAddReq = new FriendAddRequest();
            message.Request.friendAddReq.FromId = User.Instance.CurrentCharacter.Id;
            message.Request.friendAddReq.FromName = User.Instance.CurrentCharacter.Name;
            message.Request.friendAddReq.ToId = friendId;
            message.Request.friendAddReq.ToName = friendName;
            NetClient.Instance.SendMessage(message);
        }

        public void SendAddFriendResponse(bool accept, FriendAddRequest request)
        {
            Debug.Log("SendAddFriendResponse");
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.friendAddRes = new FriendAddResponse();
            message.Response.friendAddRes.Result = accept ? Result.Success : Result.Failed;
            message.Response.friendAddRes.Errormsg = accept ? "Added Successfully" : "Failed";
            message.Response.friendAddRes.Request = request;
            NetClient.Instance.SendMessage(message);
        }

        private void OnAddFriendRequest(object sender, FriendAddRequest request)
        {
            var confirm = MessageBox.Show(string.Format("{0} Request To Add You As Friend", request.FromName), "Friend Invitation", MessageBoxType.Confirm, "Accept", "Refuse");
            confirm.OnYes = () =>
            {
                SendAddFriendResponse(true, request);
            };
            confirm.OnNo = () =>
            {
                SendAddFriendResponse(false, request);
            };
        }

        private void OnAddFriendResponse(object sender, FriendAddResponse response)
        {
            if (response.Result == Result.Success)
                MessageBox.Show(response.Errormsg, "Add Friend");
            else
                MessageBox.Show(response.Errormsg, "Add Friend");
        }

        private void OnFriendList(object sender, FriendListResponse response)
        {
            Debug.Log("OnFriendList");
            FriendManager.Instance.friends = response.Friends;
            if (OnFriendUpdate != null) OnFriendUpdate();
        }

        public void SendRemoveFriendRequest(int friendshipId, int friendCharaId)
        {
            Debug.Log("SendRemoveFriend");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.friendRemove = new FriendRemoveRequest();
            message.Request.friendRemove.Id = friendshipId;
            message.Request.friendRemove.friendId = friendCharaId;
            NetClient.Instance.SendMessage(message);
        }

        private void OnRemoveFriend(object sender, FriendRemoveResponse response)
        {
            if (response.Result == Result.Success)
                MessageBox.Show("Succeeded","Delete Friends");
            else
                MessageBox.Show("Failed", "Delete Friends", MessageBoxType.Error);
        }
    }
}
