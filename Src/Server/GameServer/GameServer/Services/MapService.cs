using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Data;
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
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapTeleportRequest>(OnMapTeleport);
        }

        public void Init()
        {
            MapManager.Instance.Init();
        }

        public void Dispose()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<MapTeleportRequest>(OnMapTeleport);
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
            conn.Session.Response.mapEntitySync = new MapEntitySyncResponse();
            conn.Session.Response.mapEntitySync.entitySyncs.Add(entitySync);
            conn.SendResponse();
        }

        private void OnMapTeleport(NetConnection<NetSession> sender, MapTeleportRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("MapTeleportRequest: Character [{0}] is trying to teleport from Teleporter:{1}", character.Id, request.teleporterId);

            if (!DataManager.Instance.Teleporters.ContainsKey(request.teleporterId))
            {
                Log.WarningFormat("Source Teleporter:{0} does not exist!", request.teleporterId);
                return;
            }
            TeleporterDefine source = DataManager.Instance.Teleporters[request.teleporterId];
            if(source.LinkTo == 0 || !DataManager.Instance.Teleporters.ContainsKey(source.LinkTo))
            {
                Log.WarningFormat("Source Teleporter:{0} has linkage to Teleporter:{1} not existing!", request.teleporterId, source.LinkTo);
                return;
            }

            TeleporterDefine target = DataManager.Instance.Teleporters[source.LinkTo];

            MapManager.Instance[source.MapID].CharacterLeave(character);
            character.Position = target.Position;
            character.Direction = target.Direction;
            Log.InfoFormat("Character[{0}] teleported from Teleport[{1}] In Map[{2}] to Teleport[{3}] in Map[{4}]", character.Id, source.ID, source.MapID, target.ID, target.MapID);
            MapManager.Instance[target.MapID].CharacterEnter(sender,character);
        }
    }
}
