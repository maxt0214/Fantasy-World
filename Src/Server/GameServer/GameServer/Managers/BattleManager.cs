using System;
using System.Collections.Generic;
using System.Text;
using Common;
using GameServer.Services;
using Network;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class BattleManager : Singleton<BattleManager>
    {
        static long bid = 0;

        public void Init()
        {

        }

        public void ProcessBattleMessage(NetConnection<NetSession> sender, CastSkillRequest request)
        {
            Log.InfoFormat("BattleManagerProcessBattleMsg: skill:{0} caster:{1} target:{2} posisiton:{3}", request.Info.skillId, request.Info.casterId, request.Info.targetId, request.Info.Position.String());
            var character = sender.Session.Character;

            var battle = MapManager.Instance[character.Info.mapId].Battle;

            battle.ProcessBattleMessage(sender,request);
        }
    }
}
