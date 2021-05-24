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
    class ChatService : Singleton<ChatService>, IDisposable
    {
        public ChatService()
        {
            MessageDistributer.Instance.Subscribe<ChatResponse>(OnChat);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ChatResponse>(OnChat);
        }

        public void Init()
        {

        }

        public void SendChat(ChatChannel sendChannel, string msg, int toId, string toName)
        {
            Debug.LogFormat("SendChat: Channel:{0} Message:{1} To:[{2}]:{3}", sendChannel, msg, toId, toName);

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.Chat = new ChatRequest();
            message.Request.Chat.Message = new ChatMessage();
            message.Request.Chat.Message.Channel = sendChannel;
            message.Request.Chat.Message.ToId = toId;
            message.Request.Chat.Message.ToName = toName;
            message.Request.Chat.Message.Message = msg;
            NetClient.Instance.SendMessage(message);
        }

        private void OnChat(object sender, ChatResponse response)
        {
            if(response.Result == Result.Success)
            {
                ChatManager.Instance.AddMessages(ChatChannel.Local, response.localMessages);
                ChatManager.Instance.AddMessages(ChatChannel.World, response.worldMessages);
                ChatManager.Instance.AddMessages(ChatChannel.System, response.systemMessages);
                ChatManager.Instance.AddMessages(ChatChannel.Private, response.privateMessages);
                ChatManager.Instance.AddMessages(ChatChannel.Team, response.teamMessages);
                ChatManager.Instance.AddMessages(ChatChannel.Guild, response.guildMessages);
            } else
            {
                ChatManager.Instance.AddSystemMessage(response.Errormsg);
            }
        }
    }
}
