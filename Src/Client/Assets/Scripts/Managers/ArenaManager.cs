using Models;
using Services;
using SkillBridge.Message;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    class ArenaManager : Singleton<ArenaManager>
    {
        public ArenaInfo Info;
        public int Round;

        private int inputId = -1;

        public ArenaManager()
        {
            inputId = InputManager.Instance.RegisterInputSource();
        }

        public void EnterArena(ArenaInfo info)
        {
            Debug.LogFormat("ArenaManager.EnterArena:{0}", info.arenaId);
            Info = info;

            if (inputId != -1)
                InputManager.Instance.EnableInputSource(inputId, true);
        }

        public void ExitArena(ArenaInfo info)
        {
            Debug.LogFormat("ArenaManager.ExitArena:{0}", info.arenaId);
            Info = null;

            if (inputId != -1)
                InputManager.Instance.EnableInputSource(inputId, false);
        }

        public void SendReady()
        {
            Debug.LogFormat("ArenaManager.SetReady:{0}", Info.arenaId);
            ArenaService.Instance.SendReadyForArena(Info.arenaId);
        }

        public void OnArenaReady(int round, ArenaInfo arenaInfo)
        {
            Debug.LogFormat("ArenaManager.OnArenaReady:{0} Round[{1}]", arenaInfo.arenaId, round);
            Round = round;
            if (UIArena.Instance != null)
                UIArena.Instance.ShowCountDown();

            if (inputId != -1)
                InputManager.Instance.EnableInputSource(inputId, true);
        }

        public void OnRoundStart(int round, ArenaInfo arenaInfo)
        {
            Debug.LogFormat("ArenaManager.OnRoundStart:{0} Round[{1}]", arenaInfo.arenaId, round);
            if (UIArena.Instance != null)
                UIArena.Instance.ShowRoundStart(round, arenaInfo);

            if (inputId != -1)
                InputManager.Instance.EnableInputSource(inputId, false);
        }

        public void OnRoundOver(int round, ArenaInfo arenaInfo)
        {
            Debug.LogFormat("ArenaManager.OnRoundOver:{0} Round[{1}]", arenaInfo.arenaId, round);
            Round = round;
            if (UIArena.Instance != null)
                UIArena.Instance.ShowRoundResult(round, arenaInfo);

            if (inputId != -1)
                InputManager.Instance.EnableInputSource(inputId, true);
        }
    }
}
