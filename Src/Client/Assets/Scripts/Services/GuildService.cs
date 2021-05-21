using Models;
using Network;
using SkillBridge.Message;
using System;
using UnityEngine;
using Managers;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Services
{
    class GuildService : Singleton<GuildService>, IDisposable
    {
        public UnityAction OnGuildUpdate;
        public UnityAction<bool> OnGuildCreated;
        public UnityAction<List<NGuildInfo>> OnGuildListBack;

        public GuildService()
        {
            MessageDistributer.Instance.Subscribe<GuildCreationResponse>(OnCreateGuild);
            MessageDistributer.Instance.Subscribe<GuildListResponse>(OnGuildList);
            MessageDistributer.Instance.Subscribe<GuildJoinRequest>(OnJoinGuildRequest);
            MessageDistributer.Instance.Subscribe<GuildJoinResponse>(OnJoinGuildResponse);
            MessageDistributer.Instance.Subscribe<GuildInfoResponse>(OnGuildInfo);
            MessageDistributer.Instance.Subscribe<GuildLeaveResponse>(OnLeaveGuild);
            MessageDistributer.Instance.Subscribe<GuildAdminResponse>(OnGuildAdmin);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<GuildCreationResponse>(OnCreateGuild);
            MessageDistributer.Instance.Unsubscribe<GuildListResponse>(OnGuildList);
            MessageDistributer.Instance.Unsubscribe<GuildJoinRequest>(OnJoinGuildRequest);
            MessageDistributer.Instance.Unsubscribe<GuildJoinResponse>(OnJoinGuildResponse);
            MessageDistributer.Instance.Unsubscribe<GuildInfoResponse>(OnGuildInfo);
            MessageDistributer.Instance.Unsubscribe<GuildLeaveResponse>(OnLeaveGuild);
            MessageDistributer.Instance.Unsubscribe<GuildAdminResponse>(OnGuildAdmin);
        }

        public void Init()
        {

        }

        public void SendGuildCreation(string name, string overview)
        {
            Debug.Log("SendGuildCreation");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildCreation = new GuildCreationRequest();
            message.Request.guildCreation.Name = name;
            message.Request.guildCreation.Overview = overview;
            NetClient.Instance.SendMessage(message);
        }

        private void OnCreateGuild(object sender, GuildCreationResponse response)
        {
            Debug.LogFormat("OnGuildCreation: {0}", response.Result);
            if(OnGuildCreated != null)
            {
                OnGuildCreated(response.Result == Result.Success);
            }
            if(response.Result == Result.Success)
            {
                GuildManager.Instance.Init(response.guildInfo);
                MessageBox.Show(string.Format("Guild [{0}] Created Successfully", response.guildInfo.guildName), "Create Guild");
            } else
            {
                MessageBox.Show(string.Format("{0}. Failed To Create Guild [{1}]", response.Errormsg, response.guildInfo.guildName), "Create Guild");
            }
        }

        public void SendGuildListRequest()
        {
            Debug.Log("SendGuildList");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildList = new GuildListRequest();
            NetClient.Instance.SendMessage(message);
        }

        private void OnGuildList(object sender, GuildListResponse response)
        {
            Debug.LogFormat("OnGuildList: {0}", response.Result);
            if (response.Result == Result.Success)
            {
                if (OnGuildListBack != null) OnGuildListBack(response.Guilds);
            }
            else
            {
                MessageBox.Show(string.Format("{0}. Failed To Retreive Guild List", response.Errormsg), "Guild List");
            }
        }

        public void SendJoinGuildRequest(int gId)
        {
            Debug.Log("SendJoinGuild");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinReq = new GuildJoinRequest();
            message.Request.guildJoinReq.Applicant = new NGuildApplicantInfo();
            message.Request.guildJoinReq.Applicant.guildId = gId;
            NetClient.Instance.SendMessage(message);
        }

        private void OnJoinGuildRequest(object sender, GuildJoinRequest request)
        {
            var prompt = MessageBox.Show(string.Format("{0} Request To Join The Guild", request.Applicant.Name), "Guild Applicant", MessageBoxType.Confirm, "Approve", "Decline");
            prompt.OnYes = () =>
            {
                SendJoinGuildResponse(true, request);
            };
            prompt.OnNo = () =>
            {
                SendJoinGuildResponse(false, request);
            };
        }

        public void SendJoinGuildResponse(bool ifAccept, GuildJoinRequest request)
        {
            Debug.Log("SendJoinGuild");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinRes = new GuildJoinResponse();
            message.Request.guildJoinRes.Result = Result.Success;
            message.Request.guildJoinRes.Applicant = request.Applicant;
            message.Request.guildJoinRes.Applicant.Result = ifAccept ? JoinGuildResult.Accept : JoinGuildResult.Reject;
            message.Request.guildJoinRes.Errormsg = ifAccept ? "Joined The Guild Successfully" : "Your Request Is Refused By The Guild";
            NetClient.Instance.SendMessage(message);
        }

        private void OnJoinGuildResponse(object sender, GuildJoinResponse response)
        {
            Debug.LogFormat("OnJoinGuild: {0}", response.Result);
            if (response.Result == Result.Success)
            {
                MessageBox.Show(response.Errormsg, "Join Guild");
            }
            else
            {
                MessageBox.Show(response.Errormsg, "Join Guild");
            }
        }

        public void SendLeaveGuild()
        {
            Debug.Log("SendLeaveGuild");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildLeave = new GuildLeaveRequest();
            NetClient.Instance.SendMessage(message);
        }

        private void OnLeaveGuild(object sender, GuildLeaveResponse response)
        {
            Debug.Log("OnLeaveGuild");
            if (response.Result == Result.Success)
            {
                GuildManager.Instance.Init(null);
                MessageBox.Show("Left The Guild Successfully", "Leave Guild");
            }
            else
            {
                MessageBox.Show(string.Format("{0}. Failed To Leave The Guild", response.Errormsg), "Create Guild");
            }
        }

        private void OnGuildInfo(object sender, GuildInfoResponse response)
        {
            Debug.LogFormat("OnGuildInfo: {0} Guild[{1}]:{2}", response.Result, response.Guild.Id, response.Guild.guildName);
            GuildManager.Instance.Init(response.Guild);
            if (OnGuildUpdate != null)
                OnGuildUpdate();
        }

        public void SendGuildApplicant(bool ifAccept, NGuildApplicantInfo applicantInfo)
        {
            Debug.Log("SendGuildApplicationDecision");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinRes = new GuildJoinResponse();
            message.Request.guildJoinRes.Result = Result.Success;
            message.Request.guildJoinRes.Applicant = applicantInfo;
            message.Request.guildJoinRes.Applicant.Result = ifAccept ? JoinGuildResult.Accept : JoinGuildResult.Reject;
            if (!ifAccept) message.Request.guildJoinRes.Errormsg = "Your Request Is Refused By The Guild";
            NetClient.Instance.SendMessage(message);
        }

        public void SendAdminCommand(GuildAdminAction action, int cid)
        {
            Debug.Log("SendGuildAdminAction");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildAdmin = new GuildAdminRequest();
            message.Request.guildAdmin.Action = action;
            message.Request.guildAdmin.Target = cid;
            NetClient.Instance.SendMessage(message);
        }

        private void OnGuildAdmin(object sender, GuildAdminResponse response)
        {
            Debug.LogFormat("OnGuildAdmin: {0} {1}", response.Request.Action, response.Result);
            MessageBox.Show(string.Format("{0} {1}", response.Result, response.Errormsg), response.Request.Action.ToString());
        }
    }
}
