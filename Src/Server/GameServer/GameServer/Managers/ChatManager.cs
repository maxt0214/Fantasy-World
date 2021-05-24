using Common;
using GameServer.Entities;
using System.Collections.Generic;
using SkillBridge.Message;
using System.Linq;
using GameServer.Services;
using Common.Utils;
using System;

namespace GameServer.Managers
{
    class ChatManager : Singleton<ChatManager>
    {
        public List<ChatMessage> System = new List<ChatMessage>();
        public List<ChatMessage> World = new List<ChatMessage>();
        public Dictionary<int, List<ChatMessage>> Local = new Dictionary<int, List<ChatMessage>>(); //map,messages
        public Dictionary<int, List<ChatMessage>> Team = new Dictionary<int, List<ChatMessage>>();
        public Dictionary<int, List<ChatMessage>> Guild = new Dictionary<int, List<ChatMessage>>();

        private double lastRemove;

        public void Init()
        {
            lastRemove = TimeUtil.timestamp;
        }

        public void AddMessage(Character from, ChatMessage msg)
        {
            msg.FromId = from.Id;
            msg.FromName = from.Name;
            msg.Time = TimeUtil.timestamp;
            switch(msg.Channel)
            {
                case ChatChannel.System:
                    AddSystemMsg(msg);
                    break;
                case ChatChannel.World:
                    AddWorldMsg(msg);
                    break;
                case ChatChannel.Local:
                    AddLocalMsg(from.Info.mapId,msg);
                    break;
                case ChatChannel.Team:
                    AddTeamMsg(from.team.Id, msg);
                    break;
                case ChatChannel.Guild:
                    AddGuildMsg(from.guild.Id, msg);
                    break;
            }
            RemoveChat();
        }

        private void AddSystemMsg(ChatMessage msg)
        {
            System.Add(msg);
        }

        private void AddWorldMsg(ChatMessage msg)
        {
            World.Add(msg);
        }

        private void AddLocalMsg(int mapId, ChatMessage msg)
        {
            if(!Local.TryGetValue(mapId, out List<ChatMessage> messages))
            {
                messages = new List<ChatMessage>();
                Local[mapId] = messages;
            }
            messages.Add(msg);
        }

        private void AddTeamMsg(int tid, ChatMessage msg)
        {
            if (!Team.TryGetValue(tid, out List<ChatMessage> messages))
            {
                messages = new List<ChatMessage>();
                Team[tid] = messages;
            }
            messages.Add(msg);
        }

        private void AddGuildMsg(int gid, ChatMessage msg)
        {
            if (!Guild.TryGetValue(gid, out List<ChatMessage> messages))
            {
                messages = new List<ChatMessage>();
                Guild[gid] = messages;
            }
            messages.Add(msg);
        }

        public int GetSystemMsg(int indx, List<ChatMessage> result)
        {
            return GetNewMsg(indx, result, System);
        }

        public int GetWorldMsg(int indx, List<ChatMessage> result)
        {
            return GetNewMsg(indx, result, World);
        }

        public int GetLocalMsg(int mid, int indx, List<ChatMessage> result)
        {
            if (!Local.TryGetValue(mid, out List<ChatMessage> messages)) return 0;
            return GetNewMsg(indx, result, messages);
        }

        public int GetTeamMsg(int tid, int indx, List<ChatMessage> result)
        {
            if (!Team.TryGetValue(tid, out List<ChatMessage> messages)) return 0;
            return GetNewMsg(indx, result, messages);
        }

        public int GetGuildMsg(int gid, int indx, List<ChatMessage> result)
        {
            if (!Guild.TryGetValue(gid, out List<ChatMessage> messages)) return 0;
            return GetNewMsg(indx, result, messages);
        }

        private int GetNewMsg(int indx, List<ChatMessage> result, List<ChatMessage> messages)
        {
            if(indx == 0)
            {
                if(messages.Count > GameDefine.MaxChatRecordCount)
                {
                    indx = messages.Count - GameDefine.MaxChatRecordCount;
                }
            }
            for(;indx < messages.Count; indx++)
            {
                result.Add(messages[indx]);
            }
            return indx;
        }

        private void RemoveChat()
        {
            var currTime = TimeUtil.timestamp;
            if (currTime - lastRemove < 1800)
                return;

            RemoveObsolete(System);
            RemoveObsolete(World);
            RemoveObsolete(Local);
            RemoveObsolete(Team);
            RemoveObsolete(Guild);
        }

        private void RemoveObsolete(Dictionary<int, List<ChatMessage>> messages)
        {
            var keys = messages.Keys.ToArray();
            foreach(var key in keys)
            {
                RemoveObsolete(messages[key]);
            }
        }

        private void RemoveObsolete(List<ChatMessage> messages)
        {
            if(messages.Count > GameDefine.MaxChatRecordCount)
            {
                List<ChatMessage> remained = new List<ChatMessage>(20);
                for (int i = messages.Count - GameDefine.MaxChatRecordCount; i < messages.Count; i++)
                {
                    remained.Add(messages[i]);
                }
                messages.Clear();
                messages = remained;
            }
        }
    }
}
