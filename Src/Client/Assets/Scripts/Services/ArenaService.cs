using Models;
using Network;
using SkillBridge.Message;
using System;
using UnityEngine;
using Managers;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Services
{
    class ArenaService : Singleton<ArenaService>, IDisposable
    {
        public ArenaService()
        {
            MessageDistributer.Instance.Subscribe<ArenaChallengeRequest>(OnArenaChallengeRequest);
            MessageDistributer.Instance.Subscribe<ArenaChallengeResponse>(OnArenaChallengeResponse);
            MessageDistributer.Instance.Subscribe<ArenaStartResponse>(OnArenaStart);
            MessageDistributer.Instance.Subscribe<ArenaOverResponse>(OnArenaOver);
            MessageDistributer.Instance.Subscribe<ArenaReadyResponse>(OnArenaReady);
            MessageDistributer.Instance.Subscribe<ArenaRoundStartResponse>(OnArenaRoundStart);
            MessageDistributer.Instance.Subscribe<ArenaRoundOverResponse>(OnArenaRoundOver);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ArenaChallengeRequest>(OnArenaChallengeRequest);
            MessageDistributer.Instance.Unsubscribe<ArenaChallengeResponse>(OnArenaChallengeResponse);
            MessageDistributer.Instance.Unsubscribe<ArenaStartResponse>(OnArenaStart);
            MessageDistributer.Instance.Unsubscribe<ArenaOverResponse>(OnArenaOver);
            MessageDistributer.Instance.Unsubscribe<ArenaReadyResponse>(OnArenaReady);
            MessageDistributer.Instance.Unsubscribe<ArenaRoundStartResponse>(OnArenaRoundStart);
            MessageDistributer.Instance.Unsubscribe<ArenaRoundOverResponse>(OnArenaRoundOver);
        }

        public void Init()
        {

        }

        public void SendArenaChallenge(int cid, string name)
        {
            Debug.LogFormat("SendArenaChallenge: Friend[{0}] Name:{1}", cid, name);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.arenaChallengeReq = new ArenaChallengeRequest();
            message.Request.arenaChallengeReq.Info = new ArenaInfo();

            message.Request.arenaChallengeReq.Info.Blue = new ArenaPlayer()
            {
                Cid = User.Instance.currentCharacter.Id,
                Name = User.Instance.currentCharacter.Name,
            };

            message.Request.arenaChallengeReq.Info.Red = new ArenaPlayer()
            {
                Cid = cid,
                Name = name,
            };
            NetClient.Instance.SendMessage(message);
        }

        public void SendArenaChallengeResponse(bool accept, ArenaChallengeRequest request)
        {
            Debug.Log("SendaArenaChallengeResponse");
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.arenaChallengeRes = new ArenaChallengeResponse();
            message.Response.arenaChallengeRes.Result = accept ? Result.Success : Result.Failed;
            message.Response.arenaChallengeRes.Errormsg = accept ? "" : "Challenge Rejected";
            message.Response.arenaChallengeRes.Info = request.Info;
            NetClient.Instance.SendMessage(message);
        }

        private void OnArenaChallengeRequest(object sender, ArenaChallengeRequest request)
        {
            var confirm = MessageBox.Show(string.Format("{0} Challenge You To Arena", request.Info.Blue), "Arena Challenge", MessageBoxType.Confirm, "Accept", "Refuse");
            confirm.OnYes = () =>
            {
                SendArenaChallengeResponse(true, request);
            };
            confirm.OnNo = () =>
            {
                SendArenaChallengeResponse(false, request);
            };
        }

        private void OnArenaChallengeResponse(object sender, ArenaChallengeResponse response)
        {
            MessageBox.Show(response.Errormsg, "Arena Challenge");
        }

        private void OnArenaStart(object sender, ArenaStartResponse response)
        {
            Debug.Log("Arena Starts");
            ArenaManager.Instance.EnterArena(response.Info);
        }

        private void OnArenaOver(object sender, ArenaOverResponse response)
        {
            Debug.Log("Arena Ends");
            ArenaManager.Instance.ExitArena(response.Info);
        }

        public void SendReadyForArena(int arenaId)
        {
            Debug.Log("Ready For Arena");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.arenaReady = new ArenaReadyRequest();
            message.Request.arenaReady.arenaId = arenaId;
            message.Request.arenaReady.entityId = User.Instance.currentCharacter.entityId;
            NetClient.Instance.SendMessage(message);
        }

        private void OnArenaReady(object sender, ArenaReadyResponse response)
        {
            ArenaManager.Instance.OnArenaReady(response.Round,response.ArenaInfo);
        }

        private void OnArenaRoundStart(object sender, ArenaRoundStartResponse response)
        {
            ArenaManager.Instance.OnRoundStart(response.Round, response.ArenaInfo);
        }

        private void OnArenaRoundOver(object sender, ArenaRoundOverResponse response)
        {
            ArenaManager.Instance.OnRoundOver(response.Round, response.ArenaInfo);
        }
    }
}
