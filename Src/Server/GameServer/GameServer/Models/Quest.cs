using Common.Data;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;

namespace GameServer.Models
{
    class Quest
    {
        private TCharacterQuest dbQuest;

        public Character Owner;
        public QuestDefine Def;
        public NQuestInfo Info;

        public QuestStatus Stat
        {
            get { return Info.Status; }
        }

        private int[] targets = {0,0,0};

        public Quest(TCharacterQuest dbq, QuestDefine def, NQuestInfo info, Character owner)
        {
            Owner = owner;
            dbQuest = dbq;
            Def = def;
            Info = info;
        }

        public void Init()
        {
            if (Def.Target1 == QuestTarget.None)
            {
                dbQuest.Status = (int)QuestStatus.Complete;
                Info.Status = QuestStatus.Complete;
            }
            else
            {
                dbQuest.Status = (int)QuestStatus.InProgress;
                Info.Status = QuestStatus.InProgress;
                if (Def.Target1 == QuestTarget.Kill || Def.Target2 == QuestTarget.Kill || Def.Target3 == QuestTarget.Kill)
                {
                    Owner.OnKill += OnKillDone;
                    targets[0] = Def.Target1Num;
                    targets[1] = Def.Target2Num;
                    targets[2] = Def.Target3Num;
                }
            }
            DBService.Instance.Save();
        }

        private void OnKillDone(CharacterDefine def)
        {
            if(Def.Target1 == QuestTarget.Kill)
            {
                if(Def.Target1ID == def.TID)
                {
                    if (targets[0] > 0)
                        targets[0]--;
                }
            }
            
            if(Def.Target2 == QuestTarget.Kill)
            {
                if (Def.Target2ID == def.TID)
                {
                    if (targets[1] > 0)
                        targets[1]--;
                }
            }

            if(Def.Target3 == QuestTarget.Kill)
            {
                if (Def.Target3ID == def.TID)
                {
                    if (targets[2] > 0)
                        targets[2]--;
                }
            }
        }

        public bool CheckDoness()
        {
            if(Stat == QuestStatus.Complete)
            {
                return true;
            } else if(Stat == QuestStatus.InProgress)
            {
                return CheckItems() && CheckKills();
            }

            return false;
        }

        private bool CheckItems()
        {
            Item item;
            if(Def.Target1 == QuestTarget.Item)
            {
                item = Owner.itemManager.GetItem(Def.Target1ID);
                if (item == null)
                    return false;
                if (item.ItemCount < Def.Target1Num)
                    return false;
            }

            if (Def.Target2 == QuestTarget.Item)
            {
                item = Owner.itemManager.GetItem(Def.Target2ID);
                if (item == null)
                    return false;
                if (item.ItemCount < Def.Target2Num)
                    return false;
            }

            if (Def.Target3 == QuestTarget.Item)
            {
                item = Owner.itemManager.GetItem(Def.Target3ID);
                if (item == null)
                    return false;
                if (item.ItemCount < Def.Target3Num)
                    return false;
            }

            return true;
        }

        private bool CheckKills()
        {
            foreach(var t in targets)
            {
                if (t > 0)
                    return false;
            }
            return true;
        }

        public void Submit()
        {
            if(CheckDoness())
            {
                dbQuest.Status = (int)QuestStatus.Finished;
                Info.Status = QuestStatus.Finished;
                DistributeAwards();
                DBService.Instance.Save();
            }
        }

        private void DistributeAwards()
        {
            if (Def.RewardGold > 0)
            {
                Owner.Gold += Def.RewardGold;
            }
            if (Def.RewardExp > 0)
            {
                Owner.Exp += Def.RewardExp;
            }

            if (Def.RewardItem1 > 0)
            {
                Owner.itemManager.AddItem(Def.RewardItem1, Def.RewardItem1Count);
            }
            if (Def.RewardItem2 > 0)
            {
                Owner.itemManager.AddItem(Def.RewardItem2, Def.RewardItem2Count);
            }
            if (Def.RewardItem3 > 0)
            {
                Owner.itemManager.AddItem(Def.RewardItem3, Def.RewardItem3Count);
            }
        }
    }
}
