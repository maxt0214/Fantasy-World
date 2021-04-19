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
            characters.Clear();
        }

        public void AddCharacter(NCharacterInfo chara)
        {
            Debug.LogFormat("AddCharacter: {0}: {1} Map: {2} Entity: {3}", chara.Id, chara.Name, chara.mapId, chara.Entity.String());
            var character = new Character(chara);
            characters[chara.Id] = character;

            if (OnCharacterEnter != null)
                OnCharacterEnter(character);
        }

        public void RemoveCharacter(int charaId)
        {
            Debug.LogFormat("RemoveCharacter:{0}", charaId);
            characters.Remove(charaId);
        }
    }
}
