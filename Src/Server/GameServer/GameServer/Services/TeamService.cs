using System;
using Common;
using Network;
using SkillBridge.Message;
using GameServer.Entities;
using GameServer.Managers;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Services
{
    class TeamService : Singleton<TeamService>
    {
        public TeamService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteRequest>(OnTeamInviteFriendRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteResponse>(OnTeamInviteFriendResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamLeaveRequest>(OnTeamMemberLeft);
        }

        public void Init()
        {
            TeamManager.Instance.Init();
        }

        private void OnTeamInviteFriendRequest(NetConnection<NetSession> sender, TeamInviteRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnTeamInviteFriendRequest: FromId:{0} FromName:{1} ToId:{2} ToName:{3}", request.FromId, request.FromName, request.ToId, request.ToName);

            if(CharacterManager.Instance.GetCharacter(request.FromId) == null)
            {
                request.FromId = sender.Session.Character.Id;
                request.FromName = sender.Session.Character.Info.Name;
            }

            NetConnection<NetSession> target = SessionManager.Instance.GetSession(request.ToId);
            if(target == null)
            {
                sender.Session.Response.teamInviteRes = new TeamInviteResponse();
                sender.Session.Response.teamInviteRes.Result = Result.Failed;
                sender.Session.Response.teamInviteRes.Errormsg = "Your Friend Is Offline";
                sender.SendResponse();
                return;
            }

            if(target.Session.Character.team != null)
            {
                sender.Session.Response.teamInviteRes = new TeamInviteResponse();
                sender.Session.Response.teamInviteRes.Result = Result.Failed;
                sender.Session.Response.teamInviteRes.Errormsg = "Your Friend Is Already In A Team";
                sender.SendResponse();
                return;
            }

            Log.InfoFormat("ForwardTeamInviteFriendRequest: FromId:{0} FromName:{1} ToId:{2} ToName:{3}", request.FromId, request.FromName, request.ToId, request.ToName);
            target.Session.Response.teamInviteReq = request;
            target.SendResponse();
        }

        private void OnTeamInviteFriendResponse(NetConnection<NetSession> sender, TeamInviteResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnTeamInviteFriendResponse: Character:{0} Result:{1} FromId:{2} ToId:{3}", character.Id, response.Result, response.Request.FromId, response.Request.ToId);
            sender.Session.Response.teamInviteRes = response;

            var requester = SessionManager.Instance.GetSession(response.Request.FromId);
            if (requester == null)
            {
                sender.Session.Response.teamInviteRes.Result = Result.Failed;
                sender.Session.Response.teamInviteRes.Errormsg = "The Inviter Is Offline";
            }
            else
            {
                if (response.Result == Result.Success)
                    TeamManager.Instance.AddTeamMember(requester.Session.Character, character);
                requester.Session.Response.teamInviteRes = response;
                requester.SendResponse();
            }
            sender.SendResponse();
        }

        private void OnTeamMemberLeft(NetConnection<NetSession> sender, TeamLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnTeamMemberLeft: Character:{0} TeamId:{1}", character.Id, request.TeamId);
            sender.Session.Response.teamLeave = new TeamLeaveResponse();
            sender.Session.Response.teamLeave.characterId = character.Id;

            if(character.team == null)
            {
                sender.Session.Response.teamLeave.Result = Result.Failed;
                sender.Session.Response.teamLeave.Errormsg = "You Are Not In A Team Yet";
            } 
            else
            {
                character.team.MemberLeft(character);
                sender.Session.Response.teamLeave.Result = Result.Success;
            }

            sender.SendResponse();
        }
    }
}
