using Common;
using GameServer.Entities;
using System.Collections.Generic;
using GameServer.Services;
using System;
using GameServer.Models;

namespace GameServer.Managers
{
    class TeamManager : Singleton<TeamManager>
    {
        public List<Team> Teams = new List<Team>();
        public Dictionary<int, Team> LeaderTeams = new Dictionary<int, Team>();

        public void Init()
        {

        }

        public Team GetTeamByLeader(int leaderId)
        {
            Team team;
            LeaderTeams.TryGetValue(leaderId, out team);
            return team;
        }

        public void AddTeamMember(Character leader, Character member)
        {
            if(leader.team == null)
            {
                leader.team = CreateTeam(leader);
            }
            leader.team.AddMember(member);
        }

        private Team CreateTeam(Character leader)
        {
            Team team;
            for(int i = 0; i < Teams.Count; i++)
            {
                team = Teams[i];
                if(team.members.Count == 0)
                {
                    team.AddMember(leader);
                    return team;
                }
            }

            team = new Team(leader);
            Teams.Add(team);
            team.Id = Teams.Count;
            return team;
        }
    }
}
