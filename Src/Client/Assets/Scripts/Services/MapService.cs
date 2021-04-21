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

        public int CurrMapId { get; private set; }

        public MapService()
        {
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(OnCharacterEnterMap);
            MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(OnCharacterLeaveMap);
        }

        public void Dispose()
        {
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
                if(User.Instance.CurrentCharacter.Id == chara.Id)
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
    }
}
