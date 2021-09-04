using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SkillBridge.Message;

namespace Entities
{
    public class Entity
    {
        public int entityId;


        public Vector3Int position;
        public Vector3Int direction;
        public int speed;
        public bool ready = true;
        public IEntityController Controller;

        private NEntity entityData;
        public NEntity EntityData
        {
            get {
                UpdateEntityData();
                return entityData;
            }
            set {
                entityData = value;
                SetEntityData(value);
            }
        }

        public Entity(NEntity entity)
        {
            SetEntityData(entity);
        }

        public virtual void OnUpdate(float delta)
        {
            if (speed != 0)
            {
                Vector3 dir = direction;
                position += Vector3Int.RoundToInt(dir * speed * delta / 100f);
            }
            entityData.Position.FromVector3Int(position);
            entityData.Direction.FromVector3Int(direction);
            entityData.Speed = speed;
        }

        public void SetEntityData(NEntity entity)
        {
            if (!ready) return;
            entityId = entity.Id;
            entityData = entity;
            position = position.FromNVector3(entity.Position);
            direction = direction.FromNVector3(entity.Direction);
            speed = entity.Speed;
        }

        protected void UpdateEntityData()
        {
            entityData.Position.FromVector3Int(position);
            entityData.Direction.FromVector3Int(direction);
            entityData.Speed = speed;
        }
    }
}
