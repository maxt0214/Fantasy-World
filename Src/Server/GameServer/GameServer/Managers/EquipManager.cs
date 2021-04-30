using System;
using System.Collections.Generic;
using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Services;
using Network;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class EquipManager : Singleton<EquipManager>
    {
        public Result EquipItem(NetConnection<NetSession> sender, int slot, int itemId, bool ifEquip)
        {
            Character chara = sender.Session.Character;
            ItemDefine itemDef;
            DataManager.Instance.Items.TryGetValue(itemId,out itemDef);
            if (itemDef == null || itemDef.Class != chara.Info.Class)
                return Result.Failed;

            UpdateEquip(chara.Data.Equips,slot,itemId,ifEquip);

            DBService.Instance.Save();
            return Result.Success;
        }

        unsafe void UpdateEquip(byte[] data, int slot, int itemId, bool ifEquip)
        {
            fixed(byte* pt = data)
            {
                int* slotPt = (int*)(pt + slot * sizeof(int));
                if (ifEquip)
                    *slotPt = itemId;
                else
                    *slotPt = 0;
            }
        }
    }
}
