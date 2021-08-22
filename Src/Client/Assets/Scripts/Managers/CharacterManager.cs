using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using SkillBridge.Message;
using Entities;
using UnityEngine;
using UnityEngine.Events;
using Models;

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

        public void AddCharacter(Character chara)
        {
            Debug.LogFormat("AddCharacter: {0}: {1} Map: {2} Entity: {3}", chara.Id, chara.Name, chara.Info.mapId, chara.Info.Entity.String());
            characters[chara.Info.EntityId] = chara;
            EntityManager.Instance.AddEntity(chara);
            if (OnCharacterEnter != null)
                OnCharacterEnter(chara);
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

        public Character GetCharacter(int eid)
        {
            Character chara;
            characters.TryGetValue(eid,out chara);
            return chara;
        }

        public Character NearestCharaFrom(Vector3Int origin, float maxRange)
        {
            float currDis = maxRange;
            Character target = null;
            foreach (var chara in characters.Values)
            {
                var toCheck = Vector3.Distance(origin, chara.position);
                if (toCheck < currDis)
                {
                    currDis = toCheck;
                    target = chara;
                }
            }

            return target;
        }

        //public Character[] NearestCharasFrom(Vector3 origin, int targetCount, float maxRange)
        //{
        //    int validTargetCount = 0;
        //    foreach(var chara in characters.Values)
        //    {
        //        if (validTargetCount >= targetCount)
        //            break;
        //        if (Vector3.Distance(origin, GameObjectTool.LogicUnitToWorld(chara.position)) < maxRange)
        //            validTargetCount++;
        //    }

        //    Character[] targets = new Character[validTargetCount];
        //    foreach (var chara in characters.Values)
        //    {
                
        //    }

        //    return targets;
        //}
    }
}
