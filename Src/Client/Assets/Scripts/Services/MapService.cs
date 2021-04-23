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
                //Every time we enter the game, the server will send back this list of characters in which the first one is our player.
                if(User.Instance.CurrentCharacter == null || User.Instance.CurrentCharacter.Id == chara.Id)
                {
                    User.Instance.CurrentCharacter = chara;
                }
                CharacterManager.Instance.AddCharacter(chara);
            }

            if(CurrMapId != response.mapId)
            {
                EnterMap(response.mapId);
                CurrMapId = response.mapId;
            }
        }

        private void OnCharacterLeaveMap(object sender, MapCharacterLeaveResponse response)
        {
            Debug.LogFormat("OnCharacterLeaveMap: CharacterID{0}", response.characterId);
            if (response.characterId != User.Instance.CurrentCharacter.Id)
                CharacterManager.Instance.RemoveCharacter(response.characterId);
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
            }
            else
                Debug.LogErrorFormat("Map with ID:{0} does not exist!", mapId);
        }

        public void SendMapEntitySync(EntityEvent entityEvent, NEntity entityData)
        {
            Debug.LogFormat("MapEntitySnc Request: Entity ID:{0} Pos:{1} Dir:{2} Spd:{3} Event:{4}", entityData.Id, entityData.Position, entityData.Direction, entityData.Speed, entityEvent);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapEntitySync = new MapEntitySyncRequest();
            message.Request.mapEntitySync.entitySync = new NEntitySync()
            {
                Id = entityData.Id,
                Event = entityEvent,
                Entity = entityData
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
    }
}
