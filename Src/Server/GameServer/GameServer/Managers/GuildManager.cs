using Common;
using GameServer.Entities;
using System.Collections.Generic;
using SkillBridge.Message;
using GameServer.Services;
using System;
using GameServer.Models;
using Common.Utils;

namespace GameServer.Managers
{
    class GuildManager : Singleton<GuildManager>
    {
        public Dictionary<int, Guild> Guilds = new Dictionary<int, Guild>();
        private HashSet<string> GuildNames = new HashSet<string>();

        public void Init()
        {
            Guilds.Clear();
            foreach(var guild in DBService.Instance.Entities.Guilds)
            {
                AddGuild(new Guild(guild));
            }
        }

        private void AddGuild(Guild guild)
        {
            Guilds.Add(guild.Id,guild);
            GuildNames.Add(guild.name);
            guild.timeStamp = TimeUtil.timestamp;
        }

        public bool GuildExisted(string name)
        {
            return GuildNames.Contains(name);
        }

        public bool CreateGuild(string name, string overview, Character chara)
        {
            DateTime now = DateTime.Now;
            TGuild dbGuild = DBService.Instance.Entities.Guilds.Create();
            dbGuild.FoundTime = now;
            dbGuild.Name = name;
            dbGuild.Overview = overview;
            dbGuild.LeaderId = chara.Id;
            dbGuild.LeaderName = chara.Name;
            DBService.Instance.Entities.Guilds.Add(dbGuild);

            Guild guild = new Guild(dbGuild);
            guild.AddMember(chara.Id,chara.Name, chara.Data.Class, chara.Data.Level, GuildTitle.President);
            chara.guild = guild;
            DBService.Instance.Save();
            chara.Data.GuildId = dbGuild.Id;
            DBService.Instance.Save();
            AddGuild(guild);

            return true;
        }

        public Guild GetGuild(int guildId)
        {
            if (guildId == 0)
                return null;
            Guild guild;
            Guilds.TryGetValue(guildId, out guild);
            return guild;
        }

        public List<NGuildInfo> GetGuildsInfo()
        {
            List<NGuildInfo> guildInfos = new List<NGuildInfo>();
            foreach(var kv in Guilds)
            {
                guildInfos.Add(kv.Value.GuildInfo(null));
            }
            return guildInfos;
        }
    }
}
