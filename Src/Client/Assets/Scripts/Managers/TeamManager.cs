using System;
using System.Collections.Generic;
using Common.Data;
using Models;
using SkillBridge.Message;

namespace Managers
{
    class TeamManager : Singleton<TeamManager>
    {
        public void Init()
        {

        }

        public void UpdateTeamInfo(NTeamInfo teamInfo)
        {
            User.Instance.teamInfo = teamInfo;
            ShowTeamUI(teamInfo != null);
        }

        public void ShowTeamUI(bool ifShow)
        {
            if(UIMain.Instance != null)
            {
                UIMain.Instance.ShowTeamUI(ifShow);
            }
        }
    }
}
