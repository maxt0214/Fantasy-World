using Common.Data;
using GameServer.Core;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;

namespace GameServer.Entities
{
    class CharacterBase : Entity
    {

        public int Id
        {
            get
            {
                return entityId;
            }
        }
        public NCharacterInfo Info;
        public CharacterDefine Define;

        public CharacterBase(Vector3Int pos, Vector3Int dir):base(pos,dir)
        {

        }

        public CharacterBase(CharacterType type, int tid, int level, Vector3Int pos, Vector3Int dir) :
           base(pos, dir)
        {
            Info = new NCharacterInfo();
            Info.Type = type;
            Info.Level = level;
            Info.Tid = tid;
            Info.Entity = EntityData;
            Define = DataManager.Instance.Characters[Info.Tid];
            Info.Name = Define.Name;
        }
    }
}
