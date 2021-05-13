using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkillBridge.Message;
using UnityEngine;

namespace Entities
{
    public class Character : Entity
    {
        public NCharacterInfo Info;

        public Common.Data.CharacterDefine Define;

        public int Id
        {
            get { return Info.Id; }
        }

        public string Name
        {
            get
            {
                if (Info.Type == CharacterType.Player)
                    return Info.Name;
                else
                    return Define.Name;
            }
        }

        public bool IsPlayer
        {
            get { return Info.Type == CharacterType.Player; }
        }

        public bool IsLocalPlayer 
        {
            get 
            {
                if (!IsPlayer) return false;
                return Id == Models.User.Instance.CurrentCharacter.Id;
            }
        }

        public Character(NCharacterInfo info) : base(info.Entity)
        {
            Info = info;
            Define = DataManager.Instance.Characters[info.ConfigId];
        }

        public void MoveForward()
        {
            Debug.LogFormat("MoveForward");
            speed = Define.Speed;
        }

        public void MoveBack()
        {
            Debug.LogFormat("MoveBack");
            speed = -Define.Speed;
        }

        public void Stop()
        {
            Debug.LogFormat("Stop");
            speed = 0;
        }

        public void SetDirection(Vector3Int direction)
        {
            Debug.LogFormat("SetDirection:{0}", direction);
            this.direction = direction;
        }

        public void SetPosition(Vector3Int position)
        {
            Debug.LogFormat("SetPosition:{0}", position);
            this.position = position;
        }
    }
}
