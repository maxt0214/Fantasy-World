using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Message;
using GameServer.Entities;

namespace GameServer.Managers
{
    class CharacterManager : Singleton<CharacterManager>
    {
        public Dictionary<int, Character> Characters = new Dictionary<int, Character>();

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
            Characters.Clear();
        }

        public Character AddCharacter(TCharacter cha)
        {
            Character character = new Character(CharacterType.Player, cha);
            EntityManager.Instance.AddEntity(cha.MapID,0,character);
            character.Info.EntityId = character.entityId;
            Characters[character.Id] = character;
            return character;
        }

        public void RemoveCharacter(int characterId)
        {
            var chara = Characters[characterId];
            EntityManager.Instance.RemoveEntity(chara.Data.MapID,0,chara);
            Characters.Remove(characterId);
        }

        public Character GetCharacter(int charaId)
        {
            Character chara;
            Characters.TryGetValue(charaId,out chara);
            return chara;
        }
    }
}
