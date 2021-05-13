using Common;
using System.Collections.Generic;
using System;
using Network;

namespace GameServer.Managers
{
    class SessionManager : Singleton<SessionManager>
    {
        public Dictionary<int, NetConnection<NetSession>> Sessions = new Dictionary<int, NetConnection<NetSession>>();

        public void AddSession(int charaId, NetConnection<NetSession> session)
        {
            Sessions[charaId] = session;
        }

        public void RemoveSession(int charaId)
        {
            Sessions.Remove(charaId);
        }

        public NetConnection<NetSession> GetSession(int charaId)
        {
            NetConnection<NetSession> session;
            Sessions.TryGetValue(charaId, out session);
            return session;
        }
    }
}
