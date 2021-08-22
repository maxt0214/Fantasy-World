using System;
using System.Collections.Generic;
using SkillBridge.Message;
using Common.Data;
using UnityEngine;
using Entities;

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

        public Character currentCharacter { get; set; }

        public NCharacterInfo CurrentCharacterInfo { get; set; }
        public MapDefine currMap { get; set; }

        public PlayerInputController currentCharacterObj { get; set; }

        public NTeamInfo teamInfo { get; set; }

        public void AddGold(int toAdd)
        {
            CurrentCharacterInfo.Gold += toAdd;
        }

        public int currRide = 0;

        public void Ride(int rideId)
        {
            if(currRide != rideId)
            {
                if(currRide != 0)
                {
                    currRide = 0;
                    currentCharacterObj.SendEntityEvent(EntityEvent.Ride);
                }
                currRide = rideId;
                currentCharacterObj.SendEntityEvent(EntityEvent.Ride, currRide);
            } else
            {
                currRide = 0;
                currentCharacterObj.SendEntityEvent(EntityEvent.Ride);
            }
        }

        public delegate void CharacterInitHandler();
        public event CharacterInitHandler OnCharacterInit;

        public void CharacterInited()
        {
            if (OnCharacterInit != null)
                OnCharacterInit();
        }
    }
}
