using Common.Data;
using GameServer.Core;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using Network;
using GameServer.Models;
using Common;
using GameServer.Entities;

namespace GameServer.Models
{
    class Chat
    {
        private Character Owner;

        public int systemIdx;
        public int worldIdx;
        public int localIdx;
        public int teamIdx;
        public int guildIdx;

        public Chat(Character owner)
        {
            Owner = owner;
        }

        public void PostProcess(NetMessageResponse response)
        {
            if(response.Chat == null)
            {
                response.Chat = new ChatResponse();
                response.Chat.Result = Result.Success;
            }
            systemIdx = ChatManager.Instance.GetSystemMsg(systemIdx, response.Chat.systemMessages);
            worldIdx = ChatManager.Instance.GetWorldMsg(worldIdx, response.Chat.worldMessages);
            localIdx = ChatManager.Instance.GetLocalMsg(Owner.Info.mapId, localIdx, response.Chat.localMessages);
            if (Owner.team != null) teamIdx = ChatManager.Instance.GetTeamMsg(Owner.team.Id, teamIdx, response.Chat.teamMessages);
            if (Owner.guild != null) guildIdx = ChatManager.Instance.GetGuildMsg(Owner.guild.Id, guildIdx, response.Chat.guildMessages);
        }
    }
}
