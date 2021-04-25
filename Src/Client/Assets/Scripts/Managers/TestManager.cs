using Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Managers
{
    class TestManager : Singleton<TestManager>
    {
        public void Init()
        {
            NPCManager.Instance.RegisterNPCEvent(NPCFunction.InvokeShop,OnNPCInvokeShop);
            NPCManager.Instance.RegisterNPCEvent(NPCFunction.InvokeDungeon,OnNPCInvokeDungeon);
        }

        private bool OnNPCInvokeShop(NPCDefine npc)
        {
            Debug.LogFormat("NPC:{0} is called to invoke the shopw page", npc.Name);
            var UITester = UIManager.Instance.Show<UITestingWindow>();
            UITester.SetTitle(npc.Name);
            return true;
        }

        private bool OnNPCInvokeDungeon(NPCDefine npc)
        {
            Debug.LogFormat("NPC:{0} is called to invoke the dungeon page", npc.Name);
            var UITester = UIManager.Instance.Show<UITestingWindow>();
            UITester.SetTitle(npc.Name);
            return true;
        }
    }
}
