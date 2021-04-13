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
                    sender.Session.User = user;

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

            TCharacter newChara = new TCharacter()
            {
                TID = (int)request.Class,
                Name = request.Name,
                Class = (int)request.Class,
                MapID = 1,
                MapPosX = 5000,
                MapPosY = 4000,
                MapPosZ = 820,
            };

            byte[] data;
            try
            {
                DBService.Instance.Entities.Characters.Add(newChara);
                sender.Session.User.Player.Characters.Add(newChara);
                DBService.Instance.Entities.SaveChanges();
            }
            catch(Exception e)
            {
                Log.ErrorFormat("Character Creation Failed In Cause Of {0}", e.Message);
                message.Response.createChar.Result = Result.Failed;
                message.Response.createChar.Errormsg = e.Message;

                data = PackageHandler.PackMessage(message);
                sender.SendData(data, 0, data.Length);
                return;
            }

            message.Response.createChar.Result = Result.Success;
            message.Response.createChar.Errormsg = "None";
            foreach (var character in sender.Session.User.Player.Characters)
            {
                NCharacterInfo cInfo = new NCharacterInfo();
                cInfo.Id = character.ID;
                cInfo.Name = character.Name;
                cInfo.Class = (CharacterClass)character.Class;
                message.Response.createChar.Characters.Add(cInfo);
            }

            data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
        }
    }
}
