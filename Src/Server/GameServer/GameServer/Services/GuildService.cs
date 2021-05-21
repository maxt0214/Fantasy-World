using Common;
using Network;
using SkillBridge.Message;
using GameServer.Entities;
using System.Linq;
using GameServer.Managers;
using System;

namespace GameServer.Services
{
    class GuildService : Singleton<GuildService>
    {
        public GuildService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildCreationRequest>(OnCreateGuild);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildListRequest>(OnGuildList);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinRequest>(OnJoinGuildRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinResponse>(OnJoinGuildResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildLeaveRequest>(OnLeaveGuild);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildAdminRequest>(OnGuildAdmin);
        }

        public void Init()
        {
            GuildManager.Instance.Init();
        }

        private void OnCreateGuild(NetConnection<NetSession> sender, GuildCreationRequest request)
        {
            var chara = sender.Session.Character;
            Log.InfoFormat("OnCreateGuildRequest: Name:{0} Character[{1}]:{2}", request.Name, chara.Id, chara.Info.Name);

            sender.Session.Response.guildCreation = new GuildCreationResponse();
            if (chara.guild != null)
            {
                sender.Session.Response.guildCreation.Result = Result.Failed;
                sender.Session.Response.guildCreation.Errormsg = "You Are Already In A Guild";
                sender.SendResponse();
                return;
            }

            if(GuildManager.Instance.GuildExisted(request.Name))
            {
                sender.Session.Response.guildCreation.Result = Result.Failed;
                sender.Session.Response.guildCreation.Errormsg = string.Format("The Guild Named {0} Already Existed", request.Name);
                sender.SendResponse();
                return;
            }

            GuildManager.Instance.CreateGuild(request.Name, request.Overview, chara);
            sender.Session.Response.guildCreation.guildInfo = chara.guild.GuildInfo(chara);
            sender.Session.Response.guildCreation.Result = Result.Success;
            sender.SendResponse();
        }

        private void OnGuildList(NetConnection<NetSession> sender, GuildListRequest request)
        {
            var chara = sender.Session.Character;
            Log.InfoFormat("OnGuildList: Character[{0}]:{1}", chara.Id, chara.Info.Name);

            sender.Session.Response.guildList = new GuildListResponse();
            sender.Session.Response.guildList.Guilds.AddRange(GuildManager.Instance.GetGuildsInfo());
            sender.Session.Response.guildList.Result = Result.Success;
            sender.SendResponse();
        }

        private void OnJoinGuildRequest(NetConnection<NetSession> sender, GuildJoinRequest request)
        {
            var chara = sender.Session.Character;
            Log.InfoFormat("OnJoinGuildRequest: GuildId:{0} Character[{1}]:{2}", request.Applicant.guildId, chara.Id, chara.Info.Name);

            var guild = GuildManager.Instance.GetGuild(request.Applicant.guildId);
            if(guild == null)
            {
                sender.Session.Response.guildJoinRes = new GuildJoinResponse();
                sender.Session.Response.guildJoinRes.Result = Result.Failed;
                sender.Session.Response.guildJoinRes.Errormsg = "The Guild Does Not Exist";
                sender.SendResponse();
            }
            request.Applicant.characterId = chara.Id;
            request.Applicant.Class = chara.Data.Class;
            request.Applicant.Name = chara.Name;
            request.Applicant.Level = chara.Data.Level;

            if(guild.ProcessApplicant(request.Applicant))
            {
                var leader = SessionManager.Instance.GetSession(guild.data.LeaderId);
                if(leader != null)
                {
                    leader.Session.Response.guildJoinReq = request;
                    leader.SendResponse();
                }
            } else
            {
                sender.Session.Response.guildJoinRes = new GuildJoinResponse();
                sender.Session.Response.guildJoinRes.Result = Result.Failed;
                sender.Session.Response.guildJoinRes.Errormsg = "You Have Already Requested To Join The Guild";
                sender.SendResponse();
            }
        }

        private void OnJoinGuildResponse(NetConnection<NetSession> sender, GuildJoinResponse response)
        {
            var chara = sender.Session.Character;
            Log.InfoFormat("OnJoinGuildResponse: GuildId:{0} Character[{1}]:{2}", response.Applicant.guildId, chara.Id, chara.Info.Name);

            var guild = GuildManager.Instance.GetGuild(response.Applicant.guildId);
            if(response.Result == Result.Success)
            {
                guild.ApproveApplicant(response.Applicant);
            }

            var requester = SessionManager.Instance.GetSession(response.Applicant.characterId);
            if (requester != null)
            {
                if(response.Applicant.Result == JoinGuildResult.Accept)
                {
                    requester.Session.Character.guild = guild;
                }
                requester.Session.Response.guildJoinRes = response;
                requester.SendResponse();
            }
            sender.Session.Response.guildJoinRes = response;
            sender.Session.Response.guildJoinRes.Errormsg = response.Applicant.Result == JoinGuildResult.Accept ? "The Applicant Successfully Joined The Guild" : "Failed To Approve The Applicant";
            sender.SendResponse();
        }

        private void OnLeaveGuild(NetConnection<NetSession> sender, GuildLeaveRequest request)
        {
            var chara = sender.Session.Character;
            Log.InfoFormat("OnLeaveGuild: GuildId:{0} Character[{1}]:{2}", chara.guild.Id, chara.Id, chara.Info.Name);
            sender.Session.Response.guildLeave = new GuildLeaveResponse();

            chara.guild.MemberLeft(chara.Id, sender.Session.Response.guildLeave);
            DBService.Instance.Save();

            sender.SendResponse();
        }

        private void OnGuildAdmin(NetConnection<NetSession> sender, GuildAdminRequest request)
        {
            var chara = sender.Session.Character;
            Log.InfoFormat("OnGuildAdmin: GuildId:{0} Target[{1}] Action:{2}", chara.guild.Id, request.Target, request.Action);
            sender.Session.Response.guildAdmin = new GuildAdminResponse();
            if(chara.guild == null)
            {
                sender.Session.Response.guildAdmin.Result = Result.Failed;
                sender.Session.Response.guildAdmin.Errormsg = "You Do Not Even Have A Guild!";
                sender.SendResponse();
            }

            chara.guild.HandleAdminAction(request.Action, request.Target, chara.Id, sender.Session.Response.guildAdmin);

            if(sender.Session.Response.guildAdmin.Result == Result.Success)
            {
                var requester = SessionManager.Instance.GetSession(request.Target);
                if (requester != null)
                {
                    requester.Session.Response.guildAdmin = new GuildAdminResponse();
                    requester.Session.Response.guildAdmin.Result = Result.Success;
                    requester.Session.Response.guildAdmin.Request = request;
                    requester.Session.Response.guildAdmin.Errormsg = "Your Are " + sender.Session.Response.guildAdmin.Errormsg;
                    requester.SendResponse();
                }
            }

            sender.Session.Response.guildAdmin.Request = request;
            sender.Session.Response.guildAdmin.Errormsg = "The Target Is " + sender.Session.Response.guildAdmin.Errormsg;
            sender.SendResponse();
        }
    }
}
