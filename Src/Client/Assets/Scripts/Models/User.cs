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

        public PlayerInputController currentCharacterObj { get; set; }

        public NTeamInfo teamInfo { get; set; }

        public void AddGold(int toAdd)
        {
            CurrentCharacter.Gold += toAdd;
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
    }
}
