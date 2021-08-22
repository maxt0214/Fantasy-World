using Battle;
using Entities;
using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Services
{
    class BattleService : Singleton<BattleService>, IDisposable
    {
        public BattleService()
        {
            MessageDistributer.Instance.Subscribe<CastSkillResponse>(OnSkillCasted);
            MessageDistributer.Instance.Subscribe<SkillHitResponse>(OnHitsReceived);
            MessageDistributer.Instance.Subscribe<BuffResponse>(OnBuff);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<BuffResponse>(OnBuff);
            MessageDistributer.Instance.Unsubscribe<SkillHitResponse>(OnHitsReceived);
            MessageDistributer.Instance.Unsubscribe<CastSkillResponse>(OnSkillCasted);
        }

        public void Init()
        {

        }

        public void SendCastSkill(int skillId, int casterId, int target, NVector3 effectLoc)
        {
            if (effectLoc == null) effectLoc = new NVector3();
            Debug.LogFormat("SendCastSkill: SkillId:{0} CasterId:{1} Target:{2} EffectLoc:{3}", skillId, casterId, target, effectLoc);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.castSkill = new CastSkillRequest();
            message.Request.castSkill.Info = new NCastSkillInfo();
            message.Request.castSkill.Info.skillId = skillId;
            message.Request.castSkill.Info.casterId = casterId;
            message.Request.castSkill.Info.targetId = target;
            message.Request.castSkill.Info.Position = effectLoc;
            NetClient.Instance.SendMessage(message);
        }

        private void OnSkillCasted(object sender, CastSkillResponse response)
        {
            Debug.LogFormat("OnSkillCasted: SkillId:{0} CasterId:{1} Target:{2} EffectLoc:{3}", response.Info.skillId, response.Info.casterId, response.Info.targetId, response.Info.Position.String());
            if(response.Result == Result.Success)
            {
                var caster = EntityManager.Instance.GetEntity(response.Info.casterId) as Creature;
                if(caster != null)
                {
                    var target = EntityManager.Instance.GetEntity(response.Info.targetId) as Creature;
                    caster.CastSkill(response.Info.skillId, target, response.Info.Position);
                }
            } else
            {
                if (User.Instance.currentCharacter.entityId == response.Info.casterId) ChatManager.Instance.AddSystemMessage(response.Errormsg);
            }
        }

        private void OnHitsReceived(object sender, SkillHitResponse response)
        {
            Debug.LogFormat("OnSkillHit: Count:{0}", response.Hits.Count);
            if(response.Result == Result.Success)
            {
                foreach(var hit in response.Hits)
                {
                    Creature caster = EntityManager.Instance.GetEntity(hit.casterId) as Creature;
                    if (caster != null)
                    {
                        caster.InitSkillHit(hit);
                    }
                }
            }
        }

        private void OnBuff(object sender, BuffResponse response)
        {
            Debug.LogFormat("OnBuff: Count:{0}", response.Buffs.Count);

            foreach(var buff in response.Buffs)
            {
                var owner = EntityManager.Instance.GetEntity(buff.ownerId) as Creature;
                if(owner != null)
                {
                    Debug.LogFormat("Buff[{0}] Owner[{1}] Action:{2}", buff.Tid, owner.Name, buff.Action);
                    owner.HandleBuffAction(buff);
                }
            }
        }
    }
}
