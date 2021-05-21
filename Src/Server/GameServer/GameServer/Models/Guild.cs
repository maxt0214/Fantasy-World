using Common;
using Common.Utils;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Models
{
    class Guild
    {
        public TGuild data;

        public int Id;

        public string name { get { return data.Name; } }

        public double timeStamp;

        public Guild(TGuild guild)
        {
            data = guild;
            Id = guild.Id;
        }

        public bool ProcessApplicant(NGuildApplicantInfo applicant)
        {
            var preApplication = DBService.Instance.Entities.GuildApplicants.FirstOrDefault(a => a.CharacterId == applicant.characterId);
            if (preApplication != null)
            {
                if (preApplication.Result == (int)JoinGuildResult.None)
                    return false;
                preApplication.Result = (int)JoinGuildResult.None;
            } else
            {
                var dbApplicant = DBService.Instance.Entities.GuildApplicants.Create();
                dbApplicant.GuildId = applicant.guildId;
                dbApplicant.CharacterId = applicant.characterId;
                dbApplicant.Class = applicant.Class;
                dbApplicant.Level = applicant.Level;
                dbApplicant.Name = applicant.Name;
                dbApplicant.ApplyTime = DateTime.Now;

                DBService.Instance.Entities.GuildApplicants.Add(dbApplicant);
                data.GuildApplicants.Add(dbApplicant);
            }
            DBService.Instance.Save();

            timeStamp = TimeUtil.timestamp;
            return true;
        }

        public bool ApproveApplicant(NGuildApplicantInfo applicant)
        {
            var preApplication = DBService.Instance.Entities.GuildApplicants.FirstOrDefault(a => a.CharacterId == applicant.characterId);
            if (preApplication == null) return false;

            preApplication.Result = (int)applicant.Result;
            if(applicant.Result == JoinGuildResult.Accept)
            {
                AddMember(applicant.characterId, applicant.Name, applicant.Class, applicant.Level, GuildTitle.None);
            }
            DBService.Instance.Save();

            timeStamp = TimeUtil.timestamp;
            return true;
        }

        public void AddMember(int cid, string name, int Class, int level, GuildTitle title)
        {
            DateTime now = DateTime.Now;
            TGuildMember member = new TGuildMember
            {
                CharacterId = cid,
                Name = name,
                Class = Class,
                Level = level,
                Title = (int)title,
                JoinTime = now,
                LastOnlineTime = now,
            };
            data.GuildMembers.Add(member);
            timeStamp = TimeUtil.timestamp;

            var chara = CharacterManager.Instance.GetCharacter(cid);
            if(chara != null)
            {
                chara.Data.GuildId = Id;
            } else
            {
                //DBService.Instance.Entities.Database.ExecuteSqlCommand("UPDATE Characters SET GuildId = @p0 WHERE ID = @p1", Id, cid);
                var dbChara = DBService.Instance.Entities.Characters.FirstOrDefault(c => c.ID == cid);
                dbChara.GuildId = Id;
            }
            timeStamp = TimeUtil.timestamp;
        }

        public void MemberLeft(int cid, GuildLeaveResponse guildLeave)
        {
            Log.InfoFormat("MemberLeftGuild: Member[{0}]", cid);

            var dbMember = data.GuildMembers.FirstOrDefault(m => m.CharacterId == cid);
            if (dbMember == null)
            {
                guildLeave.Result = Result.Failed;
                guildLeave.Errormsg = "Member Does Not Exist";
                return;
            }

            if(dbMember.Title == (int)GuildTitle.President)
            {
                guildLeave.Result = Result.Failed;
                guildLeave.Errormsg = "You Are The Guild Leader. Please Transfer Your Position To Other Member First";
                return;
            }

            Leave(dbMember);
            DBService.Instance.Save();
            guildLeave.Result = Result.Success;
        }

        private void Leave(TGuildMember dbMember)
        {
            var chara = CharacterManager.Instance.GetCharacter(dbMember.CharacterId);
            if (chara != null)
            {
                chara.Data.GuildId = 0;
                chara.guild = null;
            }
            else
            {
                var dbChara = DBService.Instance.Entities.Characters.FirstOrDefault(c => c.ID == dbMember.CharacterId);
                dbChara.GuildId = 0;
            }
            DBService.Instance.Entities.GuildMembers.Remove(dbMember);
            timeStamp = TimeUtil.timestamp;
        }

        public NGuildInfo GuildInfo(Character from)
        {
            var info = new NGuildInfo()
            {
                Id = Id,
                guildName = name,
                Overview = data.Overview,
                leaderId = data.LeaderId,
                leaderName = data.LeaderName,
                foundTime = (long)TimeUtil.GetTimestamp(data.FoundTime),
                memberCount = data.GuildMembers.Count,
            };

            if(from != null)
            {
                info.Members.AddRange(GetMemberInfos());
                if (from.Id == data.LeaderId)
                    info.Applicants.AddRange(GetApplicantInfos());
            }

            return info;
        }

        private List<NGuildMemberInfo> GetMemberInfos()
        {
            var members = new List<NGuildMemberInfo>();

            foreach(var member in data.GuildMembers)
            {
                var memberInfo = new NGuildMemberInfo()
                {
                    Id = member.Id,
                    characterId = member.CharacterId,
                    Title = (GuildTitle)member.Title,
                    joinTime = (long)TimeUtil.GetTimestamp(member.JoinTime),
                    lastOnlineTime = (long)TimeUtil.GetTimestamp(member.LastOnlineTime),
                };

                var character = CharacterManager.Instance.GetCharacter(member.CharacterId);
                if(character != null)
                {
                    memberInfo.Info = character.GetBasicInfo();
                    memberInfo.Status = 1;
                    member.Level = character.Data.Level;
                    member.Name = character.Data.Name;
                    member.LastOnlineTime = DateTime.Now;
                } else
                {
                    memberInfo.Info = GetMemberInfo(member);
                    memberInfo.Status = 0;
                }
                members.Add(memberInfo);
            }
            return members;
        }

        private NCharacterInfo GetMemberInfo(TGuildMember member)
        {
            return new NCharacterInfo()
            {
                Id = member.CharacterId,
                Name = member.Name,
                Class = (CharacterClass)member.Class,
                Level = member.Level,
            };
        }

        private List<NGuildApplicantInfo> GetApplicantInfos()
        {
            var applicants = new List<NGuildApplicantInfo>();

            foreach(var applicant in data.GuildApplicants)
            {
                if (applicant.Result != (int)JoinGuildResult.None) continue;

                applicants.Add(new NGuildApplicantInfo()
                {
                    characterId = applicant.CharacterId,
                    guildId = applicant.GuildId,
                    Class = applicant.Class,
                    Level = applicant.Level,
                    Name = applicant.Name,
                    Result = (JoinGuildResult)applicant.Result,
                });
            }

            return applicants;
        }

        private TGuildMember GetDBMember(int cid)
        {
            foreach(var member in data.GuildMembers)
            {
                if(member.CharacterId == cid)
                {
                    return member;
                }
            }
            return null;
        }

        public void HandleAdminAction(GuildAdminAction action, int tid, int sid, GuildAdminResponse guildAdmin)
        {
            var target = GetDBMember(tid);
            var source = GetDBMember(sid);

            if(source == null)
            {
                guildAdmin.Result = Result.Failed;
                guildAdmin.Errormsg = "You Are Not In This Guild";
                return;
            }

            if (target == null)
            {
                guildAdmin.Result = Result.Failed;
                guildAdmin.Errormsg = "The Target Is Not In This Guild";
                return;
            }

            switch(action)
            {
                case GuildAdminAction.Kickout:
                    if (source.Title == (int)GuildTitle.None || target.Title > (int)GuildTitle.None && source.Title == (int)GuildTitle.VicePresident)
                    {
                        guildAdmin.Result = Result.Failed;
                        guildAdmin.Errormsg = "You Do Not Have Power To Do So";
                        return;
                    }
                    if (source.Title == (int)GuildTitle.President && target.Title == (int)GuildTitle.President)
                    {
                        guildAdmin.Result = Result.Failed;
                        guildAdmin.Errormsg = "The Leader! Not Sure Why You Wanna Kick Your Self Out, But You Cannot";
                        return;
                    }
                    Leave(target);
                    guildAdmin.Errormsg = " Kicked Out";
                    break;
                case GuildAdminAction.Demote:
                    if(source.Title != (int)GuildTitle.President)
                    {
                        guildAdmin.Result = Result.Failed;
                        guildAdmin.Errormsg = "You Do Not Have Power To Do So";
                        return;
                    }
                    if(target.Title == (int)GuildTitle.None)
                    {
                        guildAdmin.Result = Result.Failed;
                        guildAdmin.Errormsg = "The Taget Already Holds No Position In The Guild";
                        return;
                    }
                    if (target.Title == (int)GuildTitle.President)
                    {
                        guildAdmin.Result = Result.Failed;
                        guildAdmin.Errormsg = "How Dare You Do This To The Leader";
                        return;
                    }
                    target.Title = (int)GuildTitle.None;
                    guildAdmin.Errormsg = " Demoted To Member";
                    break;
                case GuildAdminAction.Promote:
                    if (source.Title != (int)GuildTitle.President)
                    {
                        guildAdmin.Result = Result.Failed;
                        guildAdmin.Errormsg = "You Do Not Have Power To Do So";
                        return;
                    }
                    if (target.Title > (int)GuildTitle.None)
                    {
                        guildAdmin.Result = Result.Failed;
                        guildAdmin.Errormsg = "The Taget Is Already Holding A Position In The Guild";
                        return;
                    }
                    target.Title = (int)GuildTitle.VicePresident;
                    guildAdmin.Errormsg = " Promoted To Vice President";
                    break;
                case GuildAdminAction.Transfer:
                    if (source.Title != (int)GuildTitle.President)
                    {
                        guildAdmin.Result = Result.Failed;
                        guildAdmin.Errormsg = "You Do Not Have Power To Do So";
                        return;
                    }
                    if (target.Title > (int)GuildTitle.President)
                    {
                        guildAdmin.Result = Result.Failed;
                        guildAdmin.Errormsg = "Your Are Already Leader";
                        return;
                    }
                    target.Title = (int)GuildTitle.President;
                    source.Title = (int)GuildTitle.None;
                    guildAdmin.Errormsg = " Promoted To Leader";
                    break;
            }

            DBService.Instance.Save();
            timeStamp = TimeUtil.timestamp;
            guildAdmin.Result = Result.Success;
        }

        public void PostProcess(Character from, NetMessageResponse response)
        {
            if (response.guildInfo == null)
            {
                response.guildInfo = new GuildInfoResponse();
                response.guildInfo.Result = Result.Success;
                response.guildInfo.Guild = GuildInfo(from);
            }
        }

        public void MemberOffline()
        {
            timeStamp = TimeUtil.timestamp;
        }
    }
}
