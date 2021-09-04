using System;
using System.Collections.Generic;
using Common;
using Network;
using SkillBridge.Message;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Models;

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

            var quest = chara.QuestManager.AcceptQuest(request.QuestId);

            if(quest != null && quest.Stat != QuestStatus.Finished)
            {
                sender.Session.Response.questAccept.Result = Result.Success;
                sender.Session.Response.questAccept.Errormsg = "";
                sender.Session.Response.questAccept.Quest = quest.Info;
            } else
            {
                sender.Session.Response.questAccept.Result = Result.Failed;
                sender.Session.Response.questAccept.Errormsg = "Invalid Quest";
            }

            sender.SendResponse();
        }

        private void OnQuestSubmit(NetConnection<NetSession> sender, QuestSubmitRequest request)
        {
            Character chara = sender.Session.Character;
            Log.InfoFormat("QuestSubmitRequest: Character{0}, QuestId:{1}", chara.Id, request.QuestId);
            var quest = sender.Session.Character.QuestManager.SubmitQuest(request.QuestId);
            SendSubmitQuest(sender, quest);
        }

        public void SendSubmitQuest(NetConnection<NetSession> sender, Quest quest)
        {
            sender.Session.Response.questSubmit = new QuestSubmitResponse();
            sender.Session.Response.questSubmit.Result = (quest.Stat == QuestStatus.Finished) ? Result.Success : Result.Failed;
            sender.Session.Response.questSubmit.Errormsg = "";
            sender.Session.Response.questSubmit.Quest = quest.Info;
            sender.SendResponse();
        }
    }
}
