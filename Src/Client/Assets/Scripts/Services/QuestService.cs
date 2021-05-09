using Models;
using Network;
using SkillBridge.Message;
using System;
using UnityEngine;
using Managers;

namespace Services
{
    class QuestService : Singleton<QuestService>, IDisposable
    {
        public QuestService()
        {
            MessageDistributer.Instance.Subscribe<QuestAccepctResponse>(OnQuestAccept);
            MessageDistributer.Instance.Subscribe<QuestSubmitResponse>(OnQuestSubmit);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<QuestAccepctResponse>(OnQuestAccept);
            MessageDistributer.Instance.Unsubscribe<QuestSubmitResponse>(OnQuestSubmit);
        }

        public bool SendQuestAccept(Quest quest)
        {
            Debug.LogFormat("QuestAcceptRequest: Quest[{0}] Name:{1}", quest.Define.ID, quest.Define.Name);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.questAccept = new QuestAccepctRequest();
            message.Request.questAccept.QuestId = quest.Define.ID;
            NetClient.Instance.SendMessage(message);
            return true;
        }

        private void OnQuestAccept(object sender, QuestAccepctResponse response)
        {
            Debug.LogFormat("OnQuestAccept: Result:{0}, Error:{1}", response.Result, response.Errormsg);
            if(response.Result == Result.Success)
            {
                QuestManager.Instance.OnQuestAccepted(response.Quest);
            } else
            {
                MessageBox.Show("Fail To Accept The Quest! Try Again!", "Error", MessageBoxType.Error);
            }
        }

        public bool SendQuestSubmit(Quest quest)
        {
            Debug.LogFormat("QuestSubmitRequest: Quest[{0}] Name:{1}", quest.Define.ID, quest.Define.Name);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.questSubmit = new QuestSubmitRequest();
            message.Request.questSubmit.QuestId = quest.Define.ID;
            NetClient.Instance.SendMessage(message);
            return true;
        }

        private void OnQuestSubmit(object sender, QuestSubmitResponse response)
        {
            Debug.LogFormat("OnQuestSubmit: Result:{0}, Error:{1}", response.Result, response.Errormsg);
            if (response.Result == Result.Success)
            {
                QuestManager.Instance.OnQuestSubmitted(response.Quest);
            }
            else
            {
                MessageBox.Show("Fail To Submit The Quest! Try Again!", "Error", MessageBoxType.Error);
            }
        }
    }
}
