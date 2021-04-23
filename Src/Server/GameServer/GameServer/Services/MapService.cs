using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;

namespace GameServer.Services
{
    class MapService : Singleton<MapService>, IDisposable
    {
        public MapService()
        {
            //MessageDistributer.Instance.Subscribe<MapCharacterEnterRequest>(OnCharacterEnterMap);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapEntitySyncRequest>(OnMapEntitySync);
        }

        public void Init()
        {
            MapManager.Instance.Init();
        }

        public void Dispose()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<MapEntitySyncRequest>(OnMapEntitySync);
            //MessageDistributer.Instance.Unsubscribe<MapCharacterEnterRequest>(OnCharacterEnterMap);
        }

        //private void OnCharacterEnterMap(NetConnection<NetSession> sender, MapCharacterEnterRequest request)
        //{

        //}

        private void OnMapEntitySync(NetConnection<NetSession> sender, MapEntitySyncRequest request)
        {
            Character chara = sender.Session.Character;
            Log.InfoFormat("OnMapEntitySync: Character ID:{0}, {1} Entity ID:{2} Event: {3} Entity:{4}", chara.Id, chara.Info.Name, request.entitySync.Id, request.entitySync.Event, request.entitySync.Entity.String());

            MapManager.Instance[chara.Info.mapId].UpdateEntity(request.entitySync);
        }

        public void SendEntityUpdate(NetConnection<NetSession> conn, NEntitySync entitySync)
        {
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.mapEntitySync = new MapEntitySyncResponse();
            message.Response.mapEntitySync.entitySyncs.Add(entitySync);

            byte[] data = PackageHandler.PackMessage(message);
            conn.SendData(data, 0, data.Length);
        }
    }
}
