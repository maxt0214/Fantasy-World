using Common.Data;
using Services;
using System;
using System.Collections.Generic;

namespace Managers
{
    class StoryManager : Singleton<StoryManager>
    {
        private int StoryID;
        private int InstanceID;

        public void Init()
        {
            NPCManager.Instance.RegisterNPCEvent(NPCFunction.InvokeDungeon,OnOpenStory);
        }

        private bool OnOpenStory(NPCDefine npc)
        {
            ShowStoryUI(npc.Param);
            return true;
        }

        public void ShowStoryUI(int id)
        {
            StoryDefine def;
            if (DataManager.Instance.Stories.TryGetValue(id, out def))
            {
                UIDungeonView ui = UIManager.Instance.Show<UIDungeonView>();
                ui.RefreshUI();
            }
        }

        public bool StartStory(int storyId)
        {
            if(DataManager.Instance.Stories.ContainsKey(storyId))
            {
                StoryService.Instance.SendStartStory(storyId);
                return true;
            }
            return false;
        }

        public void OnStoryStarted(int storyId, int instanceId)
        {
            StoryID = storyId;
            InstanceID = instanceId;
        }

        public void FinishStory()
        {
            StoryService.Instance.SendStoryOver(StoryID,InstanceID);
        }

        public void OnStoryOver(int storyId)
        {

        }
    }
}
