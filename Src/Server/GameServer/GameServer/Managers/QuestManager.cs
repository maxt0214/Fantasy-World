using Common;
using GameServer.Entities;
using GameServer.Models;
using System.Linq;
using System.Collections.Generic;
using SkillBridge.Message;
using Network;
using System;
using Common.Data;
using GameServer.Services;

namespace GameServer.Managers
{
    class QuestManager
    {
        private Character Owner;

        public Dictionary<int, Quest> Quests = new Dictionary<int, Quest>();

        public QuestManager(Character owner)
        {
            Owner = owner;
        }

        public void InitQuests(List<NQuestInfo> list)
        {
            foreach(var quest in Owner.Data.Quests)
            {
                var info = GetQuestInfo(quest);
                list.Add(info);
                Quests.Add(quest.QuestID, new Quest(quest, DataManager.Instance.Quests[quest.QuestID], info, Owner));
            }
        }

        private NQuestInfo GetQuestInfo(TCharacterQuest quest)
        {
            return new NQuestInfo()
            {
                QuestId = quest.QuestID,
                QuestGuid = quest.Id,
                Status = (QuestStatus)quest.Status,
                Targets = new int[3] { quest.Target1, quest.Target2, quest.Target3 }
            };
        }

        public Quest AcceptQuest(int questId)
        {
            QuestDefine quest;
            if (DataManager.Instance.Quests.TryGetValue(questId, out quest))
                return CreateQuest(questId, quest);
            else
                return null;
        }

        private Quest CreateQuest(int questId, QuestDefine questDef)
        {
            if(!Quests.TryGetValue(questId, out Quest quest))
            {
                var dbQuest = DBService.Instance.Entities.CharacterQuests.Create();
                dbQuest.QuestID = questDef.ID;

                quest = new Quest(dbQuest,questDef,GetQuestInfo(dbQuest),Owner);
                Quests.Add(questId,quest);
                quest.Init();
                Owner.Data.Quests.Add(dbQuest);
                DBService.Instance.Save();
                return quest;
            }
            return quest;
        }

        public Quest SubmitQuest(int questId)
        {
            Quest quest;
            if (Quests.TryGetValue(questId, out quest))
            {
                quest.Submit();
                DBService.Instance.Save();
                return quest;
            }
            return quest;
        }
    }
}
