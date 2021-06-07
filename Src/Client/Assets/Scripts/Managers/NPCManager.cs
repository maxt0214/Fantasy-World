using System;
using System.Collections.Generic;
using Common.Data;
using UnityEngine;

namespace Managers
{
    class NPCManager : Singleton<NPCManager>
    {
        public delegate bool NPCActionHandler(NPCDefine npc);

        public Dictionary<NPCFunction, NPCActionHandler> npcEvents = new Dictionary<NPCFunction, NPCActionHandler>();
        public Dictionary<int, Vector3> npcPositions = new Dictionary<int, Vector3>();

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
            NPCDefine def;
            DataManager.Instance.NPCs.TryGetValue(npcID, out def);
            return def;
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

        public void UpdateNPCPosition(int npc, Vector3 pos)
        {
            npcPositions[npc] = pos;
        }

        public Vector3 GetNPCPosition(int npc)
        {
            Vector3 pos;
            npcPositions.TryGetValue(npc, out pos);
            return pos;
        }
    }
}
