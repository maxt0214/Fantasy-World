using Common;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;

namespace GameServer.Models
{
    class Team
    {
        public int Id;
        public Character Leader;

        public List<Character> members = new List<Character>();

        public int timeStamp;

        public Team(Character leader)
        {
            AddMember(leader);
        }

        public void AddMember(Character member)
        {
            if(members.Count == 0)
            {
                Leader = member;
            }
            members.Add(member);
            member.team = this;
            timeStamp = Time.timestamp;
        }

        public void MemberLeft(Character member)
        {
            Log.InfoFormat("Member[{0}]:{1} Left The Team[{2}]", member.Id, member.Info.Name, Id);
            members.Remove(member);
            if(member == Leader)
            {
                if (members.Count > 0)
                    Leader = members[0];
                else
                    Leader = null;
            }
            member.team = null;
            timeStamp = Time.timestamp;
        }

        public void PostProcess(NetMessageResponse response)
        {
            if(response.teamInfo == null)
            {
                response.teamInfo = new TeamInfoResponse();
                response.teamInfo.Result = Result.Success;
                response.teamInfo.teamInfo = new NTeamInfo();
                response.teamInfo.teamInfo.Id = Id;
                response.teamInfo.teamInfo.Leader = Leader.Id;
                foreach(var member in members)
                {
                    response.teamInfo.teamInfo.Members.Add(member.GetBasicInfo());
                }
            }
        }
    }
}
