using System;
using System.Collections.Generic;
using SkillBridge.Message;
using Common.Data;
using UnityEngine;

namespace Models
{
    class User : Singleton<User>
    {
        NUserInfo userInfo;


        public NUserInfo Info
        {
            get { return userInfo; }
        }


        public void SetupUserInfo(NUserInfo info)
        {
            userInfo = info;
        }

        public NCharacterInfo CurrentCharacter { get; set; }
        public MapDefine currMap { get; set; }

        public GameObject currentCharacterObj { get; set; }

        public NTeamInfo teamInfo { get; set; }

        public void AddGold(int toAdd)
        {
            CurrentCharacter.Gold += toAdd;
        }
    }
}
