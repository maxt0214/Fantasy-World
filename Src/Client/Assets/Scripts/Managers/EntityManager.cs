using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities;
using SkillBridge.Message;

namespace Managers
{
    interface IEntityNotify {
        void OnEntityRemoved();
        void OnEntityChange(Entity entity);
        void OnEntityEvent(EntityEvent entityEvent, int param);
    }
    class EntityManager : Singleton<EntityManager>
    {
        Dictionary<int, Entity> entities = new Dictionary<int, Entity>();
        Dictionary<int, IEntityNotify> notifiers = new Dictionary<int, IEntityNotify>();

        public void RegisterEntityChangeNotify(int entityId, IEntityNotify notify)
        {
            notifiers[entityId] = notify;
        }

        public void AddEntity(Entity entity)
        {
            entities[entity.entityId] = entity;
        }

        public void RemoveEntity(NEntity entity)
        {
            entities.Remove(entity.Id);
            if(notifiers.ContainsKey(entity.Id))
            {
                notifiers[entity.Id].OnEntityRemoved();
                notifiers.Remove(entity.Id);
            }
        }

        public void OnEntitySync(NEntitySync entitySync)
        {
            Entity entity;
            entities.TryGetValue(entitySync.Id,out entity);
            if(entity != null)
            {
                if (entitySync.Entity != null)
                    entity.EntityData = entitySync.Entity;
                if(notifiers.ContainsKey(entitySync.Id))
                {
                    notifiers[entitySync.Id].OnEntityChange(entity);
                    notifiers[entitySync.Id].OnEntityEvent(entitySync.Event, entitySync.Param);
                }
            }
        }

        public Entity GetEntity(int eid)
        {
            Entity entity;
            entities.TryGetValue(eid, out entity);
            return entity;
        }
    }
}
