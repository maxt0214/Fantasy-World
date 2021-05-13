using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using SkillBridge.Message;
using Entities;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    class CharacterManager : Singleton<CharacterManager>, IDisposable
    {
        public Dictionary<int, Character> characters = new Dictionary<int, Character>();

        public UnityAction<Character> OnCharacterEnter;
        public UnityAction<Character> OnCharacterLeave;

        public CharacterManager()
        {

        }

        public void Dispose()
        {
            
        }

        public void Init()
        {

        }

        public void Clear()
        {
            int[] keys = characters.Keys.ToArray();
            foreach (var key in keys)
            {
                Instance.RemoveCharacter(key);
            }
            characters.Clear();
        }

        public void AddCharacter(NCharacterInfo chara)
        {
            Debug.LogFormat("AddCharacter: {0}: {1} Map: {2} Entity: {3}", chara.Id, chara.Name, chara.mapId, chara.Entity.String());
            var character = new Character(chara);
            characters[chara.EntityId] = character;
            EntityManager.Instance.AddEntity(character);
            if (OnCharacterEnter != null)
                OnCharacterEnter(character);
        }

        public void RemoveCharacter(int entityId)
        {
            Debug.LogFormat("RemoveCharacter:{0}", entityId);
            if (characters.ContainsKey(entityId))
            {
                EntityManager.Instance.RemoveEntity(characters[entityId].Info.Entity);
                if (OnCharacterLeave != null)
                    OnCharacterLeave(characters[entityId]);
                characters.Remove(entityId);
            }
        }
    }
}
