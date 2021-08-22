using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkillBridge.Message;
using UnityEngine;
using Models;
using Common.Data;
using Managers;
using Entities;

namespace Services
{
    class MapService : Singleton<MapService>, IDisposable
    {

        public int CurrMapId { get; set; }

        public MapService()
        {
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(OnCharacterEnterMap);
            MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(OnCharacterLeaveMap);
            MessageDistributer.Instance.Subscribe<MapEntitySyncResponse>(OnMapEntitySync);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<MapEntitySyncResponse>(OnMapEntitySync);
            MessageDistributer.Instance.Unsubscribe<MapCharacterLeaveResponse>(OnCharacterLeaveMap);
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(OnCharacterEnterMap);
        }

        public void Init()
        {

        }

        private void OnCharacterEnterMap(object sender, MapCharacterEnterResponse response)
        {
            Debug.LogFormat("OnCharacterEnterMap:Map:{0} Count:{1}", response.mapId, response.Characters.Count);

            foreach(var chara in response.Characters)
            {
                if(User.Instance.CurrentCharacterInfo == null || (chara.Type == CharacterType.Player && User.Instance.CurrentCharacterInfo.Id == chara.Id)) //local player
                {
                    User.Instance.CurrentCharacterInfo = chara;
                    if (User.Instance.currentCharacter == null)
                        User.Instance.currentCharacter = new Character(chara);
                    else
                        User.Instance.currentCharacter.UpdateInfo(chara);

                    User.Instance.CharacterInited();
                    CharacterManager.Instance.AddCharacter(User.Instance.currentCharacter);
                    continue;
                }
                CharacterManager.Instance.AddCharacter(new Character(chara)); //Oter Characters
            }

            if(CurrMapId != response.mapId)
            {
                EnterMap(response.mapId);
                CurrMapId = response.mapId;
            }
        }

        private void OnCharacterLeaveMap(object sender, MapCharacterLeaveResponse response)
        {
            Debug.LogFormat("OnCharacterLeaveMap: Character EntityID{0}", response.entityId);
            if (response.entityId != User.Instance.CurrentCharacterInfo.EntityId)
                CharacterManager.Instance.RemoveCharacter(response.entityId);
            else
                CharacterManager.Instance.Clear();
        }

        private void EnterMap(int mapId)
        {
            if (DataManager.Instance.Maps.ContainsKey(mapId))
            {
                var map = DataManager.Instance.Maps[mapId];
                User.Instance.currMap = map;
                SceneManager.Instance.LoadScene(map.Resource);
                SoundManager.Instance.PlayMusic(map.Music);
            }
            else
                Debug.LogErrorFormat("Map with ID:{0} does not exist!", mapId);
        }

        public void SendMapEntitySync(EntityEvent entityEvent, NEntity entityData, int param)
        {
            Debug.LogFormat("MapEntitySnc Request: Entity ID:{0} Pos:{1} Dir:{2} Spd:{3} Event:{4}", entityData.Id, entityData.Position, entityData.Direction, entityData.Speed, entityEvent);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapEntitySync = new MapEntitySyncRequest();
            message.Request.mapEntitySync.entitySync = new NEntitySync()
            {
                Id = entityData.Id,
                Event = entityEvent,
                Entity = entityData,
                Param = param,
            };
            NetClient.Instance.SendMessage(message);
        }

        private void OnMapEntitySync(object sender, MapEntitySyncResponse response)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("MapEntitySyncResponse: NumOfEntity:{0}", response.entitySyncs.Count);
            sb.AppendLine();
            foreach(var entity in response.entitySyncs)
            {
                EntityManager.Instance.OnEntitySync(entity);
                sb.AppendFormat("   [{0}] EVENT:{1} Entity:{2}", entity.Id, entity.Event, entity.Entity.String());
                sb.AppendLine();
            }

            Debug.Log(sb.ToString());
        }

        public void TeleportFrom(int teleID)
        {
            Debug.LogFormat("MapTeleportRequest: local player enters Teleporter:{0}", teleID);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapTeleport = new MapTeleportRequest();
            message.Request.mapTeleport.teleporterId = teleID;
            NetClient.Instance.SendMessage(message);
        }
    }
}
