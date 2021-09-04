using GameServer.Core;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Battle
{
    class Battle
    {
        public Map Map;

        public Dictionary<int, Creature> Entities = new Dictionary<int, Creature>();

        Queue<NCastSkillInfo> Actions = new Queue<NCastSkillInfo>();
        List<NCastSkillInfo> CastInfos = new List<NCastSkillInfo>();
        List<NHitInfo> HitInfos = new List<NHitInfo>();
        List<NBuffInfo> BuffInfos = new List<NBuffInfo>();

        List<Creature> DeathPool = new List<Creature>();

        public Battle(Map map)
        {
            Map = map;
        }

        public void ProcessBattleMessage(NetConnection<NetSession> sender, CastSkillRequest request)
        {
            var character = sender.Session.Character;
            if(request.Info != null)
            {
                if (character.entityId != request.Info.casterId)
                    return;

                Actions.Enqueue(request.Info);
            }
        }

        public void Update()
        {
            CastInfos.Clear();
            HitInfos.Clear();
            BuffInfos.Clear();
            if (Actions.Count > 0)
            {
                var action = Actions.Dequeue();
                ExecuteAction(action);
            }

            UpdateEntities();
            BroadcastActionMessages();
        }

        private void UpdateEntities()
        {
            DeathPool.Clear();
            foreach (var kv in Entities)
            {
                kv.Value.Update();
                if (kv.Value.IsDead())
                    DeathPool.Add(kv.Value);
            }

            foreach (var dead in DeathPool)
            {
                LeaveBattle(dead);
            }
        }

        private void ExecuteAction(NCastSkillInfo action)
        {
            BattleContext context = new BattleContext(this);
            context.Caster = EntityManager.Instance.GetCreature(action.casterId);
            context.Target = EntityManager.Instance.GetCreature(action.targetId);
            context.CastingSkill = action;
            if (action.Position != null) context.Position = new Vector3Int(action.Position.X, action.Position.Y, action.Position.Z);
            if (context.Caster != null) JoinBattle(context.Caster);
            if (context.Target != null) JoinBattle(context.Target);

            context.Caster.CastSkill(context, action.skillId);
        }

        public void JoinBattle(Creature joiner)
        {
            Entities[joiner.entityId] = joiner;
        }

        public void LeaveBattle(Creature leaver)
        {
            Entities.Remove(leaver.entityId);
        }

        public void AddCastInfo(NCastSkillInfo castSkillInfo)
        {
            CastInfos.Add(castSkillInfo);
        }

        public void AddHitInfo(NHitInfo hitInfo)
        {
            HitInfos.Add(hitInfo);
        }

        public void AddBuffAction(NBuffInfo buffInfo)
        {
            BuffInfos.Add(buffInfo);
        }

        private void BroadcastActionMessages()
        {
            if (HitInfos.Count == 0 && BuffInfos.Count == 0 && CastInfos.Count == 0) return;
            NetMessageResponse response = new NetMessageResponse();

            if (CastInfos.Count > 0)
            {
                response.castSkill = new CastSkillResponse();
                response.castSkill.Infoes.AddRange(CastInfos);
                response.castSkill.Result = Result.Success;
                response.castSkill.Errormsg = "";
            }

            if (HitInfos.Count > 0)
            {
                response.skillHits = new SkillHitResponse();
                response.skillHits.Hits.AddRange(HitInfos);
                response.skillHits.Result = Result.Success;
                response.skillHits.Errormsg = "";
            }

            if (BuffInfos.Count > 0)
            {
                response.buffRes = new BuffResponse();
                response.buffRes.Buffs.AddRange(BuffInfos);
                response.buffRes.Result = Result.Success;
                response.buffRes.Errormsg = "";
            }

            Map.BroadcastBattleResponse(response);
        }

        public List<Creature> FindMapEntitiesInRange(Vector3Int pos, int aoeRange)
        {
            return EntityManager.Instance.GetMapEntitiesInRange<Creature>(Map.ID, pos, aoeRange);
        }
    }
}
