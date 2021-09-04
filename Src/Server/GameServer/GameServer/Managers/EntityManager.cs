using Common;
using GameServer.Core;
using GameServer.Entities;
using System;
using System.Collections.Generic;

namespace GameServer.Managers
{
    class EntityManager : Singleton<EntityManager>
    {
        private int idx = 0;
        public Dictionary<int,Entity> AllEntities = new Dictionary<int, Entity>();
        public Dictionary<int, List<Entity>> MapEntities = new Dictionary<int, List<Entity>>();

        public int CalcMapIdx(int mid, int instanceId)
        {
            return mid * 10000 + instanceId;
        }

        public void AddEntity(int mapId, int instanceId, Entity entity)
        {
            entity.EntityData.Id = ++idx;
            AllEntities.Add(entity.entityId, entity);

            AddEntitiesToMap(mapId, instanceId, entity);
        }

        public void AddEntitiesToMap(int mapId, int instanceId, Entity entity)
        {
            List<Entity> entities = null;
            var idx = CalcMapIdx(mapId,instanceId);
            if (!MapEntities.TryGetValue(idx, out entities))
            {
                entities = new List<Entity>();
                MapEntities[idx] = entities;
            }
            entities.Add(entity);
        }

        public void RemoveEntity(int mapId, int instanceID, Entity entity)
        {
            AllEntities.Remove(entity.entityId);
            RemoveEntityInMap(mapId, instanceID, entity);
        }

        public void RemoveEntityInMap(int mid, int instanceID, Entity entity)
        {
            var idx = CalcMapIdx(mid, instanceID);
            MapEntities[idx].Remove(entity);
        }

        public Entity GetEntity(int eid)
        {
            Entity entity;
            AllEntities.TryGetValue(eid,out entity);
            return entity;
        }

        public Creature GetCreature(int eid)
        {
            return GetEntity(eid) as Creature;
        }

        public List<T> GetMapEntities<T>(int mapId, int instanceID, Predicate<Entity> match) where T : Creature
        {
            List<T> res = new List<T>();
            var idx = CalcMapIdx(mapId,instanceID);
            foreach(var entity in MapEntities[idx])
            {
                if (entity is T && match.Invoke(entity))
                    res.Add((T)entity);
            }
            return res;
        }

        public List<T> GetMapEntitiesInRange<T>(int mapId, Vector3Int pos, int range, int instanceID = 0) where T : Creature
        {
            return GetMapEntities<T>(mapId, instanceID, entity =>
            {
                T creature = entity as T;
                return creature.DistanceTo(pos) < range;
            });
        }
    }
}
