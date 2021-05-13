using Common;
using Network;
using SkillBridge.Message;
using GameServer.Entities;
using GameServer.Managers;
using System.Linq;

namespace GameServer.Services
{
    class FriendService : Singleton<FriendService>
    {
        public FriendService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendAddRequest>(OnFriendAddRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendAddResponse>(OnFriendAddResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendRemoveRequest>(OnFriendRemove);
        }

        public void Init()
        {

        }

        private void OnFriendAddRequest(NetConnection<NetSession> sender, FriendAddRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnFriendAddRequest: FromId:{0} FromName:{1} ToId:{2} ToName:{3}", request.FromId, request.FromName, request.ToId, request.ToName);

            if(request.ToId == 0)
            {
                foreach(var chara in CharacterManager.Instance.Characters)
                {
                    if(chara.Value.Data.Name == request.ToName)
                    {
                        request.ToId = chara.Key;
                        break;
                    }
                }
            }
            NetConnection<NetSession> friendSession = null;
            if(request.ToId > 0)
            {
                if(character.FriendManager.GetFriendInfo(request.ToId) != null)
                {
                    sender.Session.Response.friendAddRes = new FriendAddResponse();
                    sender.Session.Response.friendAddRes.Result = Result.Failed;
                    sender.Session.Response.friendAddRes.Errormsg = "The Player Is Already Your Friend";
                    sender.SendResponse();
                    return;
                }
                friendSession = SessionManager.Instance.GetSession(request.ToId);
            }
            if(friendSession == null)
            {
                sender.Session.Response.friendAddRes = new FriendAddResponse();
                sender.Session.Response.friendAddRes.Result = Result.Failed;
                sender.Session.Response.friendAddRes.Errormsg = "The Player Does Not Exist Or Is Offline";
                sender.SendResponse();
                return;
            }

            Log.InfoFormat("ForwardAddFriendRequest: FromId:{0} FromName:{1} ToId:{2} ToName:{3}", request.FromId, request.FromName, request.ToId, request.ToName);
            friendSession.Session.Response.friendAddReq = request;
            friendSession.SendResponse();
        }

        private void OnFriendAddResponse(NetConnection<NetSession> sender, FriendAddResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnFriendAddResponse: Character:{0} Result:{1} FromId:{2} ToId:{3}", character.Id, response.Result, response.Request.FromId, response.Request.ToId);
            sender.Session.Response.friendAddRes = response;

            var requester = SessionManager.Instance.GetSession(response.Request.FromId);
            if(requester == null)
            {
                sender.Session.Response.friendAddRes.Result = Result.Failed;
                sender.Session.Response.friendAddRes.Errormsg = "The Requester Is Offline";
            } else
            {
                if(response.Result == Result.Success)
                {
                    character.FriendManager.AddFriend(requester.Session.Character);
                    requester.Session.Character.FriendManager.AddFriend(character);
                    DBService.Instance.Save();
                }
                requester.Session.Response.friendAddRes = response;
                requester.SendResponse();
            }
            sender.SendResponse();
        }

        private void OnFriendRemove(NetConnection<NetSession> sender, FriendRemoveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnFriendRemove: Character:{0} FriendRelationId:{1}", character.Id, request.Id);
            sender.Session.Response.friendRemove = new FriendRemoveResponse();
            sender.Session.Response.friendRemove.Id = request.Id;

            if(character.FriendManager.RemoveFriendByID(request.Id))
            {
                sender.Session.Response.friendRemove.Result = Result.Success;
                var friendSession = SessionManager.Instance.GetSession(request.friendId);
                if(friendSession == null)
                {
                    friendSession.Session.Character.FriendManager.RemoveFriendByFriendID(character.Id);
                } else
                {
                    RemoveFriend(character.Id, request.friendId);
                }
            } else
                sender.Session.Response.friendRemove.Result = Result.Failed;

            sender.SendResponse();
        }

        private void RemoveFriend(int charaId, int friendId)
        {
            var toRemove = DBService.Instance.Entities.CharacterFriends.FirstOrDefault(v => v.TCharacterID == charaId && v.FriendID == friendId);
            if(toRemove != null)
            {
                DBService.Instance.Entities.CharacterFriends.Remove(toRemove);
            }
        }
    }
}
