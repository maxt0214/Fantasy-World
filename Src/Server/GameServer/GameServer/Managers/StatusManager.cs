using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using Network;

namespace GameServer.Managers
{
    class StatusManager
    {
        Character Owner;

        private List<NStatus> Status { get; set; }
        public bool HasStatus { get { return Status.Count > 0; } }

        public StatusManager(Character owner)
        {
            Owner = owner;
            Status = new List<NStatus>();
        }

        public void AddStatus(StatusType type, int id, int value, StatusAction action)
        {
            Status.Add(new NStatus() { Type = type, Id = id, Value = value, Action = action });
        }

        public void ChangeGoldStatus(int goldAmount)
        {
            if(goldAmount > 0)
            {
                AddStatus(StatusType.Money, 0, goldAmount, StatusAction.Add);
            }
            if(goldAmount < 0)
            {
                AddStatus(StatusType.Money, 0, -goldAmount, StatusAction.Delete);
            }
        }

        public void ChangeExpStatus(int exp)
        {
            AddStatus(StatusType.Exp, 0, exp, StatusAction.Add);
        }

        public void ChangeLevelStatus(int lv)
        {
            AddStatus(StatusType.Lv, 0, lv, StatusAction.Add);
        }

        public void ChangeItemStatus(int id, int count, StatusAction action)
        {
            AddStatus(StatusType.Item, id, count, action);
        }

        public void PostProcess(NetMessageResponse response)
        {
            if (response.statusNotify == null)
                response.statusNotify = new StatusNotify();
            foreach(var stat in Status)
            {
                response.statusNotify.Status.Add(stat);
            }
            Status.Clear();
        }
    }
}
