using System;
using System.Collections.Generic;
using Models;
using Network;
using SkillBridge.Message;
using UnityEngine;

namespace Services
{
    class StatusService : Singleton<StatusService>, IDisposable
    {
        public delegate bool StatusNotifyhandler(NStatus status);

        private Dictionary<StatusType, StatusNotifyhandler> statusEvents = new Dictionary<StatusType, StatusNotifyhandler>();
        HashSet<StatusNotifyhandler> actions = new HashSet<StatusNotifyhandler>();

        public StatusService()
        {
            MessageDistributer.Instance.Subscribe<StatusNotify>(OnStatusNotify);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<StatusNotify>(OnStatusNotify);
        }

        public void Init()
        {

        }

        public void RegisterStatusNotify(StatusType type, StatusNotifyhandler handler)
        {
            if (!actions.Add(handler)) return;

            if(!statusEvents.ContainsKey(type))
            {
                statusEvents[type] = handler;
            } else
            {
                statusEvents[type] += handler;
            }
        }

        private void OnStatusNotify(object sender, StatusNotify message)
        {
            foreach(var status in message.Status)
            {
                Notify(status);
            }
        }

        private void Notify(NStatus status)
        {
            Debug.LogFormat("Status: Type[{0}] ID:{1} Value:{2} Action:{3}", status.Type, status.Id, status.Value, status.Action);

            if(status.Type == StatusType.Money)
            {
                if (status.Action == StatusAction.Add)
                    User.Instance.AddGold(status.Value);
                else if(status.Action == StatusAction.Delete)
                    User.Instance.AddGold(-status.Value);
            }

            StatusNotifyhandler handler;
            if(statusEvents.TryGetValue(status.Type, out handler))
            {
                handler(status);
            }
        }
    }
}
