using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Network;
using UnityEngine;
using Models;
using SkillBridge.Message;
using Managers;

namespace Services
{
    class UserService : Singleton<UserService>, IDisposable
    {
        public UnityEngine.Events.UnityAction<Result, string> OnLogin;
        public UnityEngine.Events.UnityAction<Result, string> OnRegister;
        public UnityEngine.Events.UnityAction<Result, string> OnCreateCharacter;

        NetMessage pendingMessage = null;
        bool connected = false;

        public UserService()
        {
            NetClient.Instance.OnConnect += OnGameServerConnect;
            NetClient.Instance.OnDisconnect += OnGameServerDisconnect;
            MessageDistributer.Instance.Subscribe<UserRegisterResponse>(OnUserRegister);
            MessageDistributer.Instance.Subscribe<UserLoginResponse>(OnUserLogin);
            MessageDistributer.Instance.Subscribe<UserCreateCharacterResponse>(OnUserCreateCharacter);
            MessageDistributer.Instance.Subscribe<UserGameEnterResponse>(OnUserEnterGame);
            MessageDistributer.Instance.Subscribe<UserGameLeaveResponse>(OnUserLeaveGame);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<UserGameLeaveResponse>(OnUserLeaveGame);
            MessageDistributer.Instance.Unsubscribe<UserGameEnterResponse>(OnUserEnterGame);
            MessageDistributer.Instance.Unsubscribe<UserCreateCharacterResponse>(OnUserCreateCharacter);
            MessageDistributer.Instance.Unsubscribe<UserLoginResponse>(OnUserLogin);
            MessageDistributer.Instance.Unsubscribe<UserRegisterResponse>(OnUserRegister);
            NetClient.Instance.OnConnect -= OnGameServerConnect;
            NetClient.Instance.OnDisconnect -= OnGameServerDisconnect;
        }

        public void Init()
        {

        }

        public void ConnectToServer()
        {
            Debug.Log("ConnectToServer() Start ");
            //NetClient.Instance.CryptKey = SessionId;
            NetClient.Instance.Init("127.0.0.1", 8000);
            NetClient.Instance.Connect();
        }


        void OnGameServerConnect(int result, string reason)
        {
            Log.InfoFormat("LoadingMesager::OnGameServerConnect :{0} reason:{1}", result, reason);
            if (NetClient.Instance.Connected)
            {
                connected = true;
                if(pendingMessage!=null)
                {
                    NetClient.Instance.SendMessage(pendingMessage);
                    pendingMessage = null;
                }
            }
            else
            {
                if (!DisconnectNotify(result, reason))
                {
                    MessageBox.Show(string.Format("NETWORK ERROR: Cannot Connect To Server！\n RESULT:{0} ERROR:{1}", result, reason), "ERROR", MessageBoxType.Error);
                }
            }
        }

        public void OnGameServerDisconnect(int result, string reason)
        {
            DisconnectNotify(result, reason);
            return;
        }

        bool DisconnectNotify(int result,string reason)
        {
            if (pendingMessage != null)
            {
                if (pendingMessage.Request.userRegister!=null)
                {
                    if (OnRegister != null)
                    {
                        OnRegister(Result.Failed, string.Format("SERVER DISCONNECT！\n RESULT:{0} ERROR:{1}", result, reason));
                    }
                }
                return true;
            }
            return false;
        }

        public void SendLogin(string user, string psw)
        {
            Debug.LogFormat("UserLoginRequest::user :{0} psw:{1}", user, psw);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.userLogin = new UserLoginRequest();
            message.Request.userLogin.User = user;
            message.Request.userLogin.Passward = psw;

            if (connected && NetClient.Instance.Connected)
            {
                pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                pendingMessage = message;
                ConnectToServer();
            }
        }

        void OnUserLogin(object sender, UserLoginResponse response)
        {
            Debug.LogFormat("OnUserLogin:{0} [{1}]", response.Result, response.Errormsg);

            if(response.Result == Result.Success) //Success. Retrieve user character info
            {
                User.Instance.SetupUserInfo(response.Userinfo);
            }

            if (OnLogin != null)
            {
                OnLogin(response.Result, response.Errormsg);
            }
        }

        public void SendRegister(string user, string psw)
        {
            Debug.LogFormat("UserRegisterRequest::user :{0} psw:{1}", user, psw);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.userRegister = new UserRegisterRequest();
            message.Request.userRegister.User = user;
            message.Request.userRegister.Passward = psw;

            if (connected && NetClient.Instance.Connected)
            {
                pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                pendingMessage = message;
                ConnectToServer();
            }
        }

        void OnUserRegister(object sender, UserRegisterResponse response)
        {
            Debug.LogFormat("OnUserRegister:{0} [{1}]", response.Result, response.Errormsg);

            if (OnRegister != null)
            {
                OnRegister(response.Result, response.Errormsg);
            }
        }

        public void SendCharacterCreation(CharacterClass charClass, string nickName)
        {
            Debug.LogFormat("UserCreateCharacterRequest::class :{0} nick name:{1}", charClass.ToString(), nickName);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.createChar = new UserCreateCharacterRequest();
            message.Request.createChar.Class = charClass;
            message.Request.createChar.Name = nickName;

            if (connected && NetClient.Instance.Connected)
            {
                pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                pendingMessage = message;
                ConnectToServer();
            }
        }

        void OnUserCreateCharacter(object sender, UserCreateCharacterResponse response)
        {
            Debug.LogFormat("OnUserCreateCharacter:{0} [{1}]", response.Result, response.Errormsg);

            if(response.Result == Result.Success)
            {
                User.Instance.Info.Player.Characters.Clear();
                User.Instance.Info.Player.Characters.AddRange(response.Characters);
            }

            if(OnCreateCharacter != null)
            {
                OnCreateCharacter(response.Result, response.Errormsg);
            }
        }

        public void SendEnterGame(int charaID)
        {
            Debug.LogFormat("UserEnterGameRequest::Character ID: {0}", charaID);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.gameEnter = new UserGameEnterRequest();
            message.Request.gameEnter.characterIdx = charaID;

            NetClient.Instance.SendMessage(message);
        }

        private void OnUserEnterGame(object sender, UserGameEnterResponse response)
        {
            Debug.LogFormat("OnUserEnterGame:{0} [{1}]", response.Result, response.Errormsg);
            if(response.Result == Result.Success)
            {
                if(response.Character != null)
                    ItemManager.Instance.Init(response.Character.Items);
            }
        }

        public void SendLeaveGame()
        {
            Debug.LogFormat("UserLeaveGameRequest::Character ID: {0} Name:{1}", User.Instance.CurrentCharacter.Id, User.Instance.CurrentCharacter.Name);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.gameLeave = new UserGameLeaveRequest();

            NetClient.Instance.SendMessage(message);
        }

        private void OnUserLeaveGame(object sender, UserGameLeaveResponse response)
        {
            MapService.Instance.CurrMapId = 0;
            User.Instance.CurrentCharacter = null;
            Debug.LogFormat("OnUserLeaveGame:{0} [{1}]", response.Result, response.Errormsg);
        }
    }
}
