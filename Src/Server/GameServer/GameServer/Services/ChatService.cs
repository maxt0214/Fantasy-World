using Common;
using Network;
using SkillBridge.Message;
using GameServer.Entities;
using System.Linq;
using GameServer.Managers;
using System;

namespace GameServer.Services
{
    class ChatService : Singleton<ChatService>
    {
        public ChatService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ChatRequest>(OnChat);
        }

        public void Init()
        {
            ChatManager.Instance.Init();
        }

        private void OnChat(NetConnection<NetSession> sender, ChatRequest request)
        {
            Character chara = sender.Session.Character;
            Log.InfoFormat("OnChat: Character[{0}] Channel:{1} Message:{2}", chara.Id, request.Message.Channel, request.Message.Message);
            if(request.Message.Channel == ChatChannel.Private)
            {
                var chatTarget = SessionManager.Instance.GetSession(request.Message.ToId);
                if(chatTarget == null)
                {
                    if (sender.Session.Response.Chat == null) sender.Session.Response.Chat = new ChatResponse();
                    sender.Session.Response.Chat.Result = Result.Failed;
                    sender.Session.Response.Chat.Errormsg = "The Other Is Offline";
                    sender.Session.Response.Chat.privateMessages.Add(request.Message);
                    sender.SendResponse();
                } else
                {
                    request.Message.FromId = chara.Id;
                    request.Message.FromName = chara.Name;

                    if (chatTarget.Session.Response.Chat == null) chatTarget.Session.Response.Chat = new ChatResponse();
                    chatTarget.Session.Response.Chat.Result = Result.Success;
                    chatTarget.Session.Response.Chat.privateMessages.Add(request.Message);
                    chatTarget.SendResponse();

                    if (sender.Session.Response.Chat == null) sender.Session.Response.Chat = new ChatResponse();
                    sender.Session.Response.Chat.Result = Result.Success;
                    sender.Session.Response.Chat.privateMessages.Add(request.Message);
                    sender.SendResponse();
                }
            } else
            {
                sender.Session.Response.Chat = new ChatResponse();
                sender.Session.Response.Chat.Result = Result.Success;
                ChatManager.Instance.AddMessage(chara,request.Message);
                sender.SendResponse();
            }
        }
    }
}
