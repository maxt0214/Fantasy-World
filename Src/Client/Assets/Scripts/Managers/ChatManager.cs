using System;
using System.Collections.Generic;
using System.Text;
using Common.Data;
using Models;
using Services;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    class ChatManager : Singleton<ChatManager>
    {
        public enum LocalChannel
        {
            All = 0,
            Local = 1,
            World = 2,
            Team = 3,
            Guild = 4,
            Private = 5,
        }

        private ChatChannel[] channelFilter = new ChatChannel[6]
        {
            ChatChannel.Local | ChatChannel.World | ChatChannel.Team | ChatChannel.Guild | ChatChannel.Private | ChatChannel.System,
            ChatChannel.Local,
            ChatChannel.World,
            ChatChannel.Team,
            ChatChannel.Guild,
            ChatChannel.Private
        };

        public UnityAction OnChat;

        public List<ChatMessage>[] Messages = new List<ChatMessage>[6] {
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>()
        };

        public LocalChannel displayChannel;
        public LocalChannel sendChannel;

        public int chatTarget;
        public string chatName;

        public ChatChannel SendChannel
        {
            get
            {
                switch(sendChannel)
                {
                    case LocalChannel.Local: return ChatChannel.Local;
                    case LocalChannel.World: return ChatChannel.World;
                    case LocalChannel.Team: return ChatChannel.Team;
                    case LocalChannel.Guild: return ChatChannel.Guild;
                    case LocalChannel.Private: return ChatChannel.Private;
                }
                return ChatChannel.Local;
            }
        }

        public void Init()
        {
            foreach(var msgs in Messages)
            {
                msgs.Clear();
            }
        }

        public void StartPrivateChat(int targetId, string targetName)
        {
            chatTarget = targetId;
            chatName = targetName;

            sendChannel = LocalChannel.Private;
            if (OnChat != null) OnChat();
        }

        public void SendChatMessage(string msg)
        {
            ChatService.Instance.SendChat(SendChannel,msg,chatTarget,chatName);
        }

        public bool SetSendChannel(LocalChannel channelSelected)
        {
            if(channelSelected == LocalChannel.Team)
            {
                if(User.Instance.teamInfo == null)
                {
                    AddSystemMessage("You Did Not Join Any Team");
                    return false;
                }
            }

            if (channelSelected == LocalChannel.Guild)
            {
                if (User.Instance.CurrentCharacterInfo.Guild == null)
                {
                    AddSystemMessage("You Did Not Join Any Guild");
                    return false;
                }
            }

            sendChannel = channelSelected;
            Debug.LogFormat("SendChannelSetTo: {0}", sendChannel.ToString());
            return true;
        }

        public void AddMessages(ChatChannel channel, List<ChatMessage> messages)
        {
            for(int ch = 0; ch < 6; ch++)
            {
                if((channelFilter[ch] & channel) == channel)
                {
                    Messages[ch].AddRange(messages);
                }
            }
            if (OnChat != null) OnChat();
        }

        public void AddSystemMessage(string msg, string from = "")
        {
            Messages[(int)LocalChannel.All].Add(new ChatMessage()
            {
                Channel = ChatChannel.System,
                Message = msg,
                FromName = from
            });
            if (OnChat != null) OnChat();
        }

        public string GetCurrentMessage()
        {
            StringBuilder sb = new StringBuilder();
            foreach(var msg in Messages[(int)displayChannel])
            {
                sb.AppendLine(FormatMessage(msg));
            }
            return sb.ToString();
        }

        private string FormatMessage(ChatMessage msg)
        {
            switch(msg.Channel)
            {
                case ChatChannel.Local:
                    return string.Format("[Local] {0} {1}", FormatFromPlayer(msg), msg.Message);
                case ChatChannel.World:
                    return string.Format("<color=cyan>[World]</color> {0} {1}", FormatFromPlayer(msg), msg.Message);
                case ChatChannel.System:
                    return string.Format("<color=yellow>[System] {0}</color>", msg.Message);
                case ChatChannel.Private:
                    return string.Format("<color=magenta>[Private]</color> {0} {1}", FormatFromPlayer(msg), msg.Message);
                case ChatChannel.Team:
                    return string.Format("<color=green>[Team]</color> {0} {1}", FormatFromPlayer(msg), msg.Message);
                case ChatChannel.Guild:
                    return string.Format("<color=blue>[Guild]</color> {0} {1}", FormatFromPlayer(msg), msg.Message);
            }
            return "";
        }

        private string FormatFromPlayer(ChatMessage msg)
        {
            if (msg.FromId == User.Instance.CurrentCharacterInfo.Id)
            {
                return string.Format("<a name=\"\" class=\"player\">[Me]</a>");
            }
            else
                return string.Format("<a name=\"c:{0}:{1}\" class=\"player\">[{1}]</a>", msg.FromId, msg.FromName);
        }
    }
}
