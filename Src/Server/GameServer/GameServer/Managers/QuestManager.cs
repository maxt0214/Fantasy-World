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
        Character Owner;
        public QuestManager(Character owner)
        {
            Owner = owner;
        }

        public void GetQuestInfos(List<NQuestInfo> list)
        {
            foreach(var quest in Owner.Data.Quests)
            {
                list.Add(GetQuestInfo(quest));
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

        public Result AcceptQuest(NetConnection<NetSession> sender, int questId)
        {
            Character chara = sender.Session.Character;

            QuestDefine quest;
            if(DataManager.Instance.Quests.TryGetValue(questId,out quest))
            {
                var dbQuest = DBService.Instance.Entities.CharacterQuests.Create();
                dbQuest.QuestID = quest.ID;

                if(quest.Target1 == QuestTarget.None)
                {
                    dbQuest.Status = (int)QuestStatus.Complete;
                } else
                {
                    dbQuest.Status = (int)QuestStatus.InProgress;
                }
                sender.Session.Response.questAccept.Quest = GetQuestInfo(dbQuest);
                chara.Data.Quests.Add(dbQuest);
                DBService.Instance.Save();
                return Result.Success;
            } else
            {
                sender.Session.Response.questAccept.Errormsg = "Quest Does not Exist";
                return Result.Failed;
            }
        }

        public Result SubmitQuest(NetConnection<NetSession> sender, int questId)
        {
            Character chara = sender.Session.Character;

            QuestDefine quest;
            if (DataManager.Instance.Quests.TryGetValue(questId, out quest))
            {
                var dbQuest = chara.Data.Quests.Where(q => q.QuestID == questId).FirstOrDefault();
                if(dbQuest != null)
                {
                    if(dbQuest.Status != (int)QuestStatus.Complete)
                    {
                        sender.Session.Response.questAccept.Errormsg = "Quest Has Not Yet Been Finished!";
                        return Result.Failed;
                    }
                    dbQuest.Status = (int)QuestStatus.Finished;
                    sender.Session.Response.questSubmit.Quest = GetQuestInfo(dbQuest);
                    DBService.Instance.Save();

                    //Reward Player
                    if(quest.RewardGold > 0)
                    {
                        chara.Gold += quest.RewardGold;
                    }
                    if(quest.RewardExp > 0)
                    {
                        //chara.Exp += quest.RewardExp;
                    }

                    if(quest.RewardItem1 > 0)
                    {
                        chara.itemManager.AddItem(quest.RewardItem1, quest.RewardItem1Count);
                    }
                    if (quest.RewardItem2 > 0)
                    {
                        chara.itemManager.AddItem(quest.RewardItem2, quest.RewardItem2Count);
                    }
                    if (quest.RewardItem3 > 0)
                    {
                        chara.itemManager.AddItem(quest.RewardItem3, quest.RewardItem3Count);
                    }
                    DBService.Instance.Save();
                    return Result.Success;
                }
                sender.Session.Response.questAccept.Errormsg = "Quest Does Not Exist [2]";//Not exist in database
                return Result.Failed;
            }
            else
            {
                sender.Session.Response.questAccept.Errormsg = "Quest Does Not Exist [1]";//Not exist in quest define
                return Result.Failed;
            }
        }
    }
}
