using System;
using System.Collections.Generic;
using Common;
using Network;
using SkillBridge.Message;
using GameServer.Entities;
using GameServer.Managers;

namespace GameServer.Services
{
    class QuestService : Singleton<QuestService>
    {
        public QuestService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<QuestAccepctRequest>(OnQuestAccept);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<QuestSubmitRequest>(OnQuestSubmit);
        }

        public void Init()
        {

        }

        private void OnQuestAccept(NetConnection<NetSession> sender, QuestAccepctRequest request)
        {
            Character chara = sender.Session.Character;
            Log.InfoFormat("QuestAcceptRequest: Character{0}, QuestId:{1}", chara.Id, request.QuestId);

            sender.Session.Response.questAccept = new QuestAccepctResponse();

            Result res = chara.QuestManager.AcceptQuest(sender, request.QuestId);
            sender.Session.Response.questAccept.Result = res;
            sender.SendResponse();
        }

        private void OnQuestSubmit(NetConnection<NetSession> sender, QuestSubmitRequest request)
        {
            Character chara = sender.Session.Character;
            Log.InfoFormat("QuestSubmitRequest: Character{0}, QuestId:{1}", chara.Id, request.QuestId);

            sender.Session.Response.questSubmit = new QuestSubmitResponse();

            Result res = chara.QuestManager.SubmitQuest(sender, request.QuestId);
            sender.Session.Response.questSubmit.Result = res;
            sender.SendResponse();
        }
    }
}
