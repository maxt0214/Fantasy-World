using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Data;

namespace Managers
{
    class NPCManager : Singleton<NPCManager>
    {
        public delegate bool NPCActionHandler(NPCDefine npc);

        public Dictionary<NPCFunction, NPCActionHandler> npcEvents = new Dictionary<NPCFunction, NPCActionHandler>();

        public void RegisterNPCEvent(NPCFunction function, NPCActionHandler action)
        {
            if(!npcEvents.ContainsKey(function))
            {
                npcEvents[function] = action;
            } else
            {
                npcEvents[function] += action;
            }
        }

        public NPCDefine GetNPCDefine(int npcID)
        {
            return DataManager.Instance.NPCs[npcID];
        }

        public bool Interactive(int npcID)
        {
            if(DataManager.Instance.NPCs.ContainsKey(npcID))
            {
                var npc = DataManager.Instance.NPCs[npcID];
                return Interact(npc);
            }
            return false;
        }

        private bool Interact(NPCDefine npc)
        {
            if(InvokeTask(npc))
            {
                return true;
            } else if(npc.Type == NPCType.Functional)
            {
                return InvokeFunction(npc);
            }
            return false;
        }

        private bool InvokeFunction(NPCDefine npc)
        {
            if (npc.Type != NPCType.Functional)
                return false;
            if (!npcEvents.ContainsKey(npc.Function))
                return false;

            return npcEvents[npc.Function](npc);
        }

        private bool InvokeTask(NPCDefine npc)
        {
            var stat = QuestManager.Instance.GetQuestStatusByNpc(npc.ID);
            if (stat == NPCQuestStatus.None)
                return false;

            return QuestManager.Instance.OpenNpcQuest(npc.ID);
        }
    }
}
