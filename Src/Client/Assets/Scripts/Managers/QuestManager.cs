using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;

namespace Managers
{
    public enum NPCQuestStatus
    {
        None = 0,
        Complete = 1,
        Available = 2,
        Incomplete = 3
    }

    public class QuestManager : Singleton<QuestManager>
    {
        public List<NQuestInfo> quesInfos;
        public Dictionary<int,Quest> allQuests = new Dictionary<int, Quest>();

        public Dictionary<int, Dictionary<NPCQuestStatus, List<Quest>>> npcQuests = new Dictionary<int, Dictionary<NPCQuestStatus, List<Quest>>>();
        public UnityAction<Quest> onQuestStatusChanged;

        public void Init(List<NQuestInfo> quests)
        {
            quesInfos = quests;
            allQuests.Clear();
            npcQuests.Clear();
            InitQuests();
        }

        private void InitQuests()
        {
            foreach (var questInfo in quesInfos)
            {
                Quest newQuest = new Quest(questInfo);
                allQuests[newQuest.Info.QuestId] = newQuest;
            }

            PopulateAvailableQuests();

            foreach(var quest in allQuests.Values)
            {
                AddNpcQuest(quest.Define.AcceptNPC, quest);
                AddNpcQuest(quest.Define.SubmitNPC, quest);
            }
        }

        private void PopulateAvailableQuests()
        {
            foreach (var kv in DataManager.Instance.Quests)
            {
                if (kv.Value.LimitClass != CharacterClass.None && kv.Value.LimitClass != User.Instance.CurrentCharacterInfo.Class)
                    continue;

                if (kv.Value.LimitLevel > User.Instance.CurrentCharacterInfo.Level)
                    continue;

                if (allQuests.ContainsKey(kv.Key))
                    continue;

                if (kv.Value.PreQuest > 0)
                {
                    Quest preQuest;
                    if (allQuests.TryGetValue(kv.Value.PreQuest, out preQuest))
                    {
                        if (preQuest.Info == null) //Have not yet accepted the pre quest
                            continue;
                        if (preQuest.Info.Status != QuestStatus.Finished) //have not yet finished
                            continue;
                    }
                    else //Have not yet reached the previous quest
                        continue;
                }
                Quest quest = new Quest(kv.Value);
                allQuests[quest.Define.ID] = quest;
            }
        }

        private void AddNpcQuest(int npcId, Quest quest)
        {
            if (!npcQuests.ContainsKey(npcId))
                npcQuests[npcId] = new Dictionary<NPCQuestStatus, List<Quest>>();

            List<Quest> availables;
            List<Quest> completes;
            List<Quest> incompletes;

            if(!npcQuests[npcId].TryGetValue(NPCQuestStatus.Available, out availables))
            {
                availables = new List<Quest>();
                npcQuests[npcId][NPCQuestStatus.Available] = availables;
            }
            if (!npcQuests[npcId].TryGetValue(NPCQuestStatus.Complete, out completes))
            {
                completes = new List<Quest>();
                npcQuests[npcId][NPCQuestStatus.Complete] = completes;
            }
            if (!npcQuests[npcId].TryGetValue(NPCQuestStatus.Incomplete, out incompletes))
            {
                incompletes = new List<Quest>();
                npcQuests[npcId][NPCQuestStatus.Incomplete] = incompletes;
            }

            if(quest.Info == null)
            {
                if(npcId == quest.Define.AcceptNPC && !npcQuests[npcId][NPCQuestStatus.Available].Contains(quest))
                {
                    npcQuests[npcId][NPCQuestStatus.Available].Add(quest);
                }
            } 
            else
            {
                if(npcId == quest.Define.SubmitNPC && quest.Info.Status == QuestStatus.Complete)
                {
                    if (!npcQuests[npcId][NPCQuestStatus.Complete].Contains(quest)) npcQuests[npcId][NPCQuestStatus.Complete].Add(quest);
                }
                if (npcId == quest.Define.SubmitNPC && quest.Info.Status == QuestStatus.InProgress)
                {
                    if (!npcQuests[npcId][NPCQuestStatus.Incomplete].Contains(quest)) npcQuests[npcId][NPCQuestStatus.Incomplete].Add(quest);
                }
            }
        }

        public NPCQuestStatus GetQuestStatusByNpc(int npcId)
        {
            Dictionary<NPCQuestStatus, List<Quest>> statQuests;
            if(npcQuests.TryGetValue(npcId, out statQuests))
            {
                if (statQuests[NPCQuestStatus.Complete].Count > 0) return NPCQuestStatus.Complete;
                if (statQuests[NPCQuestStatus.Available].Count > 0) return NPCQuestStatus.Available;
                if (statQuests[NPCQuestStatus.Incomplete].Count > 0) return NPCQuestStatus.Incomplete;
            }
            return NPCQuestStatus.None;
        }

        public bool OpenNpcQuest(int npcId)
        {
            Dictionary<NPCQuestStatus, List<Quest>> statQuests;
            if (npcQuests.TryGetValue(npcId, out statQuests))
            {
                if (statQuests[NPCQuestStatus.Complete].Count > 0) return ShowQuestDialog(statQuests[NPCQuestStatus.Complete].First());
                if (statQuests[NPCQuestStatus.Available].Count > 0) return ShowQuestDialog(statQuests[NPCQuestStatus.Available].First());
                if (statQuests[NPCQuestStatus.Incomplete].Count > 0) return ShowQuestDialog(statQuests[NPCQuestStatus.Incomplete].First());
            }
            return false;
        }

        private bool ShowQuestDialog(Quest quest)
        {
            if(quest.Info == null || quest.Info.Status == QuestStatus.Complete)
            {
                var dlg = UIManager.Instance.Show<UIQuestDialog>();
                dlg.SetQuest(quest);
                dlg.OnClose += OnQuestDialogClose;
                return true;
            }
            if (quest.Info != null)
            {
                if (!string.IsNullOrEmpty(quest.Define.DialogIncomplete))
                {
                    MessageBox.Show(quest.Define.DialogIncomplete);
                    return true;
                }
            }
            return false;
        }

        private void OnQuestDialogClose(UIWindow sender, UIWindow.WindowResult result)
        {
            UIQuestDialog dlg = (UIQuestDialog)sender;
            if(result == UIWindow.WindowResult.Confirm)
            {
                if (dlg.quest.Info == null)
                    QuestService.Instance.SendQuestAccept(dlg.quest);
                else if (dlg.quest.Info.Status == QuestStatus.Complete)
                    QuestService.Instance.SendQuestSubmit(dlg.quest);
            } else if(result == UIWindow.WindowResult.No)
            {
                MessageBox.Show(dlg.quest.Define.DialogDeny);
            }
        }

        private Quest RefreshQuestStatus(NQuestInfo questInfo)
        {
            npcQuests.Clear();
            Quest res;
            if(allQuests.ContainsKey(questInfo.QuestId))
            {
                allQuests[questInfo.QuestId].Info = questInfo;
                res = allQuests[questInfo.QuestId];
            } else
            {
                res = new Quest(questInfo);
                allQuests[questInfo.QuestId] = res;
            }

            PopulateAvailableQuests();

            foreach (var quest in allQuests.Values)
            {
                AddNpcQuest(quest.Define.AcceptNPC, quest);
                AddNpcQuest(quest.Define.SubmitNPC, quest);
            }

            if (onQuestStatusChanged != null) onQuestStatusChanged(res);
            return res;
        }

        public void OnQuestAccepted(NQuestInfo questInfo)
        {
            var quest = RefreshQuestStatus(questInfo);
            MessageBox.Show(quest.Define.DialogAccept);
        }

        public void OnQuestSubmitted(NQuestInfo questInfo)
        {
            var quest = RefreshQuestStatus(questInfo);
            MessageBox.Show(quest.Define.DialogFinish);
        }
    }
}
