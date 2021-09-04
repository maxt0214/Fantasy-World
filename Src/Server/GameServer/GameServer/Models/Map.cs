using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Message;

using Common;
using Common.Data;

using Network;
using GameServer.Managers;
using GameServer.Entities;
using GameServer.Services;

namespace GameServer.Models
{
    class Map
    {
        internal class MapCharacter
        {
            public NetConnection<NetSession> connection;
            public Character character;

            public MapCharacter(NetConnection<NetSession> conn, Character cha)
            {
                connection = conn;
                character = cha;
            }
        }

        public int ID
        {
            get { return Define.ID; }
        }
        public int InstanceID { get; set; }

        internal MapDefine Define;

        public Battle.Battle Battle;

        /// <summary>
        /// Paired as character Id, Character
        /// </summary>
        Dictionary<int, MapCharacter> MapCharacters = new Dictionary<int, MapCharacter>();

        SpawnManager spawnManager = new SpawnManager();
        public MonsterManager monsterManager = new MonsterManager();

        internal Map(MapDefine define, int instanceId)
        {
            Define = define;
            InstanceID = instanceId;
            monsterManager.Init(this);
            spawnManager.Init(this);
            Battle = new Battle.Battle(this);
        }

        public void Update()
        {
            spawnManager.Update();
            Battle.Update();
        }

        /// <summary>
        /// Character Enters the map. Will notify all characters in the map that the character is in
        /// </summary>
        /// <param name="character"></param>
        internal void CharacterEnter(NetConnection<NetSession> conn, Character character)
        {
            Log.InfoFormat("CharacterEnter: Map:{0} Online characterId:{1}", Define.ID, character.Info.Id);
            AddCharaToMap(conn, character);

            conn.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            conn.Session.Response.mapCharacterEnter.mapId = Define.ID;

            foreach (var kv in MapCharacters)
            {
                conn.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.character.Info); //Sending other characters to this client
                if (kv.Value.character != character) AddCharacterEnterMap(kv.Value.connection, character.Info); //Notifying other characters of this character
            }
            foreach (var kv in monsterManager.monsters)
            {
                conn.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.Info);
            }
            conn.SendResponse();
        }

        public void AddCharaToMap(NetConnection<NetSession> conn, Character character)
        {
            character.Info.mapId = ID;
            character.OnEnteredMap(this);
            if(!MapCharacters.ContainsKey(character.Id))
            {
                Log.InfoFormat("AddCharacterToMap: Map:{0} Online characterId:{1}", Define.ID, character.Info.Id);
                MapCharacters[character.Id] = new MapCharacter(conn, character);
            }
        }

        internal void CharacterLeave(Character character)
        {
            character.OnLeftMap(this);
            Log.InfoFormat("CharacterLeave: Map:{0} Online characterId:{1}", Define.ID, character.Id);
            
            foreach (var kv in MapCharacters)
            {
                SendCharacterLeaveMap(kv.Value.connection, character.Info);
            }
            MapCharacters.Remove(character.Id);
        }

        void AddCharacterEnterMap(NetConnection<NetSession> conn, NCharacterInfo character)
        {
            if(conn.Session.Response.mapCharacterEnter == null)
            {
                conn.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
                conn.Session.Response.mapCharacterEnter.mapId = ID;
            }
            conn.Session.Response.mapCharacterEnter.Characters.Add(character);
            conn.SendResponse();
        }

        private void SendCharacterLeaveMap(NetConnection<NetSession> conn, NCharacterInfo character)
        {
            Log.InfoFormat("SendCharacterLeaveMap: To Character[{0}]:{1} Map:{2} CharacterLeft[{3}]:{4}", conn.Session.Character.Id, conn.Session.Character.Info.Name, Define.ID, character.Id, character.Name);
            conn.Session.Response.mapCharacterLeave = new MapCharacterLeaveResponse();
            conn.Session.Response.mapCharacterLeave.entityId = character.EntityId;
            conn.SendResponse();
        }

        internal void UpdateEntity(NEntitySync entitySync)
        {
            foreach(var kv in MapCharacters)
            {
                if(kv.Value.character.entityId == entitySync.Id)
                {
                    kv.Value.character.Position = entitySync.Entity.Position;
                    kv.Value.character.Direction = entitySync.Entity.Direction;
                    kv.Value.character.Speed = entitySync.Entity.Speed;
                    if(entitySync.Event == EntityEvent.Ride)
                    {
                        kv.Value.character.ride = entitySync.Param;
                    }
                } else
                {
                    MapService.Instance.SendEntityUpdate(kv.Value.connection, entitySync);
                }
            }
        }

        internal void MonsterEnter(Monster monster)
        {
            Log.InfoFormat("MonsterEnter: Map:{0} MonsterId:{1}", ID, monster.Id);
            monster.OnEnteredMap(this);
            foreach (var kv in MapCharacters)
            {
                AddCharacterEnterMap(kv.Value.connection, monster.Info);
            }
        }

        internal void MonsterLeave(Monster monster)
        {
            Log.InfoFormat("MonsterLeave: Map:{0} MonsterId:{1}", ID, monster.Id);
            monster.OnLeftMap(this);
            foreach (var kv in MapCharacters)
            {
                SendCharacterLeaveMap(kv.Value.connection, monster.Info);
            }
        }

        internal void BroadcastBattleResponse(NetMessageResponse response)
        {
            foreach (var kv in MapCharacters)
            {
                if (response.castSkill != null)
                    kv.Value.connection.Session.Response.castSkill = response.castSkill;
                if (response.skillHits != null)
                    kv.Value.connection.Session.Response.skillHits = response.skillHits;
                if (response.buffRes != null)
                    kv.Value.connection.Session.Response.buffRes = response.buffRes;
                kv.Value.connection.SendResponse();
            }
        }
    }
}
