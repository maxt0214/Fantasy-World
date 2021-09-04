using Common;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Models;
using Network;
using SkillBridge.Message;
using System;

namespace GameServer.Services
{
    class ArenaService : Singleton<ArenaService>
    {
        public ArenaService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ArenaChallengeRequest>(OnArenaChallengeRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ArenaChallengeResponse>(OnArenaChallengeResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ArenaReadyRequest>(OnArenaReadyRequest);
        }

        public void Init()
        {
            ArenaManager.Instance.Init();
        }

        private void OnArenaChallengeRequest(NetConnection<NetSession> sender, ArenaChallengeRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnArenaChallengeRequest: Red[{0}] Name:{1} Blue[{2}] Name:{3}", request.Info.Red.Cid, request.Info.Red.Name, request.Info.Blue.Cid, request.Info.Blue.Name);

            if (request.Info.Blue.Cid != character.Id)
            {
                request.Info.Blue.Cid = character.Id;
                request.Info.Blue.Name = character.Name;
            }

            NetConnection<NetSession> red = SessionManager.Instance.GetSession(request.Info.Red.Cid);
            if (red == null)
            {
                sender.Session.Response.arenaChallengeRes = new ArenaChallengeResponse();
                sender.Session.Response.arenaChallengeRes.Result = Result.Failed;
                sender.Session.Response.arenaChallengeRes.Errormsg = "Your Friend Is Offline";
                sender.SendResponse();
                return;
            }

            Log.InfoFormat("ForwardArenaChallengeRequest: Red[{0}] Name:{1} Blue[{2}] Name:{3}", request.Info.Red.Cid, request.Info.Red.Name, request.Info.Blue.Cid, request.Info.Blue.Name);
            red.Session.Response.arenaChallengeReq = request;
            red.SendResponse();
        }

        private void OnArenaChallengeResponse(NetConnection<NetSession> sender, ArenaChallengeResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnArenaChallengeResponse: Red[{0}] Name:{1} Blue[{2}] Name:{3}", response.Info.Red.Cid, response.Info.Red.Name, response.Info.Blue.Cid, response.Info.Blue.Name);
            sender.Session.Response.arenaChallengeRes = response;

            var blue = SessionManager.Instance.GetSession(response.Info.Blue.Cid);
            if (blue == null)
            {
                sender.Session.Response.arenaChallengeRes.Result = Result.Failed;
                sender.Session.Response.arenaChallengeRes.Errormsg = "The Inviter Is Offline";
            }

            if (response.Result == Result.Failed)
            {
                blue.Session.Response.arenaChallengeRes = response;
                blue.Session.Response.arenaChallengeRes.Result = Result.Failed;
                blue.Session.Response.arenaChallengeRes.Errormsg = "The Inviter Is Offline";
                blue.SendResponse();
                return;
            }

            var arena = ArenaManager.Instance.NewArena(response.Info, blue, sender);
            SendArenaStart(blue, sender, arena);
        }

        private void SendArenaStart(NetConnection<NetSession> blue, NetConnection<NetSession> red, Arena arena)
        {
            var arenaStart = new ArenaStartResponse();
            arenaStart.Result = Result.Success;
            arenaStart.Errormsg = "";
            arenaStart.Info = arena.Info;
            blue.Session.Response.arenaStart = arenaStart;
            blue.SendResponse();
            red.Session.Response.arenaStart = arenaStart;
            red.SendResponse();
        }

        private void OnArenaReadyRequest(NetConnection<NetSession> sender, ArenaReadyRequest request)
        {
            Log.InfoFormat("Arena[{0}]: EntityID[{1}] Ready!", request.arenaId, request.entityId);
            var arena = ArenaManager.Instance.GetArena(request.arenaId);
            if(arena != null)
            {
                arena.SetEntityReady(request.entityId);
            }
        }

        public void SendArenaReady(Arena arena)
        {
            var arenaReady = new ArenaReadyResponse();
            arenaReady.Round = arena.Round;
            arenaReady.ArenaInfo = arena.Info;

            arena.Red.Session.Response.arenaReady = arenaReady;
            arena.Red.SendResponse();
            arena.Blue.Session.Response.arenaReady = arenaReady;
            arena.Blue.SendResponse();
        }

        public void SendArenaRoundStart(Arena arena)
        {
            var roundStart = new ArenaRoundStartResponse();
            roundStart.Round = arena.Round;
            roundStart.ArenaInfo = arena.Info;

            arena.Red.Session.Response.arenaRoundStart = roundStart;
            arena.Red.SendResponse();
            arena.Blue.Session.Response.arenaRoundStart = roundStart;
            arena.Blue.SendResponse();
        }

        public void SendArenaRoundOver(Arena arena)
        {
            var roundOver = new ArenaRoundOverResponse();
            roundOver.Round = arena.Round;
            roundOver.ArenaInfo = arena.Info;

            arena.Red.Session.Response.arenaRoundOver = roundOver;
            arena.Red.SendResponse();
            arena.Blue.Session.Response.arenaRoundOver = roundOver;
            arena.Blue.SendResponse();
        }

        public void SendArenaOver(Arena arena)
        {
            var arenaSettled = new ArenaOverResponse();
            arenaSettled.Info = arena.Info;
            arenaSettled.Result = Result.Success;
            arenaSettled.Errormsg = "";

            arena.Red.Session.Response.arenaOver = arenaSettled;
            arena.Red.SendResponse();
            arena.Blue.Session.Response.arenaOver = arenaSettled;
            arena.Blue.SendResponse();
        }
    }
}
