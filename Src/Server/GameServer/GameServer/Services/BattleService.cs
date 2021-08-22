using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;

namespace GameServer.Services
{
    class BattleService : Singleton<BattleService>
    {
        public BattleService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<CastSkillRequest>(OnCastSkill);
        }

        public void Init()
        {

        }

        private void OnCastSkill(NetConnection<NetSession> sender, CastSkillRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnCastSkillRequestSent: SkillId:{0} CasterId:{1} Target:{2} EffectLoc:{3}", request.Info.skillId, request.Info.casterId, request.Info.targetId, request.Info.Position);

            BattleManager.Instance.ProcessBattleMessage(sender,request);
        }
    }
}
