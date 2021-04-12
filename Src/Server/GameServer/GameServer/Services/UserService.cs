using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Network;
using SkillBridge.Message;
using GameServer.Entities;


namespace GameServer.Services
{
    class UserService : Singleton<UserService>
    {
        public UserService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserRegisterRequest>(OnRegister);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserLoginRequest>(OnLogin);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserCreateCharacterRequest>(OnCreateCharacter);
        }

        public void Init()
        {

        }

        void OnLogin(NetConnection<NetSession> sender, UserLoginRequest request)
        {
            Log.InfoFormat("User Login Request: User: {0}, Pass: {1}", request.User, request.Passward);
            //Generate message to send to client
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.userLogin = new UserLoginResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user == null) //User not existed. Login failed
            {
                message.Response.userLogin.Result = Result.Failed;
                message.Response.userLogin.Errormsg = "User Not Existed!";
            }
            else //User existed. Check credential
            {
                if(user.Password != request.Passward)
                {
                    message.Response.userLogin.Result = Result.Failed;
                    message.Response.userLogin.Errormsg = "Incorrect Password!";
                } else
                {
                    message.Response.userLogin.Result = Result.Success;
                    message.Response.userLogin.Errormsg = "None";
                    //Retrieve user info
                    message.Response.userLogin.Userinfo = new NUserInfo();
                    message.Response.userLogin.Userinfo.Id = 1;
                    message.Response.userLogin.Userinfo.Player = new NPlayerInfo();
                    message.Response.userLogin.Userinfo.Player.Id = user.Player.ID;
                    foreach (var character in user.Player.Characters) 
                    {
                        NCharacterInfo cInfo = new NCharacterInfo();
                        cInfo.Id = character.ID;
                        cInfo.Name = character.Name;
                        cInfo.Class = (CharacterClass)character.Class;
                        message.Response.userLogin.Userinfo.Player.Characters.Add(cInfo);
                    }
                }
            }
            //Send message to client
            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
        }

        void OnRegister(NetConnection<NetSession> sender, UserRegisterRequest request)
        {
            Log.InfoFormat("User Register Request: User: {0}, Pass: {1}", request.User, request.Passward);
            //Generate message to send to client
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.userRegister = new UserRegisterResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user != null) //If user exists, we do not create a duplicate
            {
                message.Response.userRegister.Result = Result.Failed;
                message.Response.userRegister.Errormsg = "User Already Existed!";
            }
            else //Create user only if not duplicated
            {
                TPlayer player = DBService.Instance.Entities.Players.Add(new TPlayer());
                DBService.Instance.Entities.Users.Add(new TUser() { Username = request.User, Password = request.Passward, Player = player });
                DBService.Instance.Entities.SaveChanges();
                message.Response.userRegister.Result = Result.Success;
                message.Response.userRegister.Errormsg = "None";
            }
            //Send message to client
            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data,0,data.Length);
        }

        void OnCreateCharacter(NetConnection<NetSession> sender, UserCreateCharacterRequest request)
        {
            Log.InfoFormat("Create Character Request: Character Class: {0}, Nick Name: {1}", request.Class.ToString(), request.Name);
            //Generate message to send to client
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.createChar = new UserCreateCharacterResponse();

            //TODO:Finish this
        }
    }
}
