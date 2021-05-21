using Models;
using Network;
using SkillBridge.Message;
using System;
using UnityEngine;
using Managers;

namespace Services
{
    class TeamService : Singleton<TeamService>, IDisposable
    {
        public void Init()
        {

        }

        public TeamService()
        {
            MessageDistributer.Instance.Subscribe<TeamInviteRequest>(OnInviteTeamRequest);
            MessageDistributer.Instance.Subscribe<TeamInviteResponse>(OnInviteTeamResponse);
            MessageDistributer.Instance.Subscribe<TeamInfoResponse>(OnTeamInfo);
            MessageDistributer.Instance.Subscribe<TeamLeaveResponse>(OnLeaveTeam);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<TeamInviteRequest>(OnInviteTeamRequest);
            MessageDistributer.Instance.Unsubscribe<TeamInviteResponse>(OnInviteTeamResponse);
            MessageDistributer.Instance.Unsubscribe<TeamInfoResponse>(OnTeamInfo);
            MessageDistributer.Instance.Unsubscribe<TeamLeaveResponse>(OnLeaveTeam);
        }

        public void SendInviteFriendToTeamRequest(int friendId, string friendName)
        {
            Debug.LogFormat("SendInviteFriendToTeamRequest: Friend[{0}] Name:{1}", friendId, friendName);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.teamInviteReq = new TeamInviteRequest();
            message.Request.teamInviteReq.FromId = User.Instance.CurrentCharacter.Id;
            message.Request.teamInviteReq.FromName = User.Instance.CurrentCharacter.Name;
            message.Request.teamInviteReq.ToId = friendId;
            message.Request.teamInviteReq.ToName = friendName;
            NetClient.Instance.SendMessage(message);
        }

        public void SendInviteFriendToTeamResponse(bool accept, TeamInviteRequest request)
        {
            Debug.Log("SendInviteFriendToTeamResponse");
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.teamInviteRes = new TeamInviteResponse();
            message.Response.teamInviteRes.Result = accept ? Result.Success : Result.Failed;
            message.Response.teamInviteRes.Errormsg = accept ? " Joined Your Team" : "Team Invitation Rejected";
            message.Response.teamInviteRes.Request = request;
            NetClient.Instance.SendMessage(message);
        }

        private void OnInviteTeamRequest(object sender, TeamInviteRequest request)
        {
            var confirm = MessageBox.Show(string.Format("{0} Invite You To The Team", request.FromName), "Team Invitation", MessageBoxType.Confirm, "Accept", "Refuse");
            confirm.OnYes = () =>
            {
                SendInviteFriendToTeamResponse(true, request);
            };
            confirm.OnNo = () =>
            {
                SendInviteFriendToTeamResponse(false, request);
            };
        }

        private void OnInviteTeamResponse(object sender, TeamInviteResponse response)
        {
            if (response.Result == Result.Success)
                MessageBox.Show(response.Request.ToName + response.Errormsg, "Team Invitation Succeeded");
            else
                MessageBox.Show(response.Errormsg, "Team Invitation Failed");
        }

        private void OnTeamInfo(object sender, TeamInfoResponse response)
        {
            Debug.Log("OnTeamInfo");
            TeamManager.Instance.UpdateTeamInfo(response.teamInfo);
        }

        public void SendLeaveTeamRequest()
        {
            if(User.Instance.teamInfo == null)
            {
                MessageBox.Show("You Are Not In A Team Yet");
                return;
            }

            Debug.Log("SendLeaveTeamRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.teamLeave = new TeamLeaveRequest();
            message.Request.teamLeave.TeamId = User.Instance.teamInfo.Id;
            message.Request.teamLeave.characterId = User.Instance.CurrentCharacter.Id;
            NetClient.Instance.SendMessage(message);
        }

        private void OnLeaveTeam(object sender, TeamLeaveResponse response)
        {
            if (response.Result == Result.Success)
            {
                TeamManager.Instance.UpdateTeamInfo(null);
                MessageBox.Show("You Have left The Team", "Leave Team", MessageBoxType.Information);
            }
            else
                MessageBox.Show(response.Errormsg, "Leave Team", MessageBoxType.Error);
        }
    }
}
