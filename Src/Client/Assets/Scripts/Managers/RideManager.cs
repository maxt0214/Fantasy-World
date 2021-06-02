using Models;
using System.Collections.Generic;
using UnityEngine.Events;
using Common.Data;
using Services;
using System;
using SkillBridge.Message;

namespace Managers
{
    class RideManager : Singleton<RideManager>
    {
        public UnityAction RideChanged;

        public List<Item> Rides = new List<Item>();

        public void AddRide(Item item)
        {
            if(item.itemDef.Type == ItemType.Ride)
            {
                Rides.Add(item);
                if (RideChanged != null) RideChanged();
            }
        }

        public void RemoveRide(Item item)
        {
            if (item.itemDef.Type == ItemType.Ride)
            {
                Rides.Remove(item);
                if (RideChanged != null) RideChanged();
            }
        }
    }
}
