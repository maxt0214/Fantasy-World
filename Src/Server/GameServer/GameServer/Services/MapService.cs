using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Network;
using SkillBridge.Message;

namespace GameServer.Services
{
    class MapService : Singleton<MapService>, IDisposable
    {
        public MapService()
        {
            //MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(OnCharacterEnterMap);
            //MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(OnCharacterLeaveMap);
        }

        public void Dispose()
        {
            //MessageDistributer.Instance.Unsubscribe<MapCharacterLeaveResponse>(OnCharacterLeaveMap);
            //MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(OnCharacterEnterMap);
        }
    }
}
