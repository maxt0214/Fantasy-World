using System;
using System.Linq;
using Common;
using Network;
using SkillBridge.Message;
using GameServer.Entities;
using GameServer.Managers;

namespace GameServer.Services
{
    class UserService : Singleton<UserService>
    {
        public UserService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserRegisterRequest>(OnRegister);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserLoginRequest>(OnLogin);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserCreateCharacterRequest>(OnCreateCharacter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameEnterRequest>(OnUserEnterGame);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameLeaveRequest>(OnUserLeaveGame);
        }

        public void Init()
        {

        }

        void OnLogin(NetConnection<NetSession> sender, UserLoginRequest request)
        {
            Log.InfoFormat("User Login Request: User: {0}, Pass: {1}", request.User, request.Passward);
            //Generate message to send to client
            sender.Session.Response.userLogin = new UserLoginResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user == null) //User not existed. Login failed
            {
                sender.Session.Response.userLogin.Result = Result.Failed;
                sender.Session.Response.userLogin.Errormsg = "User Not Existed!";
            }
            else //User existed. Check credential
            {
                if(user.Password != request.Passward)
                {
                    sender.Session.Response.userLogin.Result = Result.Failed;
                    sender.Session.Response.userLogin.Errormsg = "Incorrect Password!";
                } else
                {
                    sender.Session.User = user;

                    sender.Session.Response.userLogin.Result = Result.Success;
                    sender.Session.Response.userLogin.Errormsg = "None";
                    //Retrieve user info
                    sender.Session.Response.userLogin.Userinfo = new NUserInfo();
                    sender.Session.Response.userLogin.Userinfo.Id = (int)user.ID;
                    sender.Session.Response.userLogin.Userinfo.Player = new NPlayerInfo();
                    sender.Session.Response.userLogin.Userinfo.Player.Id = user.Player.ID;
                    foreach (var character in user.Player.Characters) 
                    {
                        NCharacterInfo cInfo = new NCharacterInfo();
                        cInfo.Id = character.ID;
                        cInfo.Name = character.Name;
                        cInfo.Type = CharacterType.Player;
                        cInfo.Class = (CharacterClass)character.Class;
                        cInfo.ConfigId = character.TID;
                        sender.Session.Response.userLogin.Userinfo.Player.Characters.Add(cInfo);
                    }
                }
            }
            //Send message to client
            sender.SendResponse();
        }

        void OnRegister(NetConnection<NetSession> sender, UserRegisterRequest request)
        {
            Log.InfoFormat("User Register Request: User: {0}, Pass: {1}", request.User, request.Passward);
            //Generate message to send to client
            sender.Session.Response.userRegister = new UserRegisterResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user != null) //If user exists, we do not create a duplicate
            {
                sender.Session.Response.userRegister.Result = Result.Failed;
                sender.Session.Response.userRegister.Errormsg = "User Already Existed!";
            }
            else //Create user only if not duplicated
            {
                TPlayer player = DBService.Instance.Entities.Players.Add(new TPlayer());
                DBService.Instance.Entities.Users.Add(new TUser() { Username = request.User, Password = request.Passward, Player = player });
                DBService.Instance.Entities.SaveChanges();
                sender.Session.Response.userRegister.Result = Result.Success;
                sender.Session.Response.userRegister.Errormsg = "None";
            }
            //Send message to client
            sender.SendResponse();
        }

        void OnCreateCharacter(NetConnection<NetSession> sender, UserCreateCharacterRequest request)
        {
            Log.InfoFormat("Create Character Request: Character Class: {0}, Nick Name: {1}", request.Class.ToString(), request.Name);
            //Generate message to send to client
            sender.Session.Response.createChar = new UserCreateCharacterResponse();

            TCharacter newChara = new TCharacter()
            {
                TID = (int)request.Class,
                Name = request.Name,
                Class = (int)request.Class,
                Level = 1,
                MapID = 1,
                MapPosX = 5000,
                MapPosY = 4000,
                MapPosZ = 820,
                Gold = 10000,
                Equips = new byte[28],
            };

            try
            {
                var bag = new TCharacterBag();
                bag.Owner = newChara;
                bag.items = new byte[0];
                bag.Unlocked = 20;
                newChara.Bag = DBService.Instance.Entities.CharacterBags.Add(bag);
                newChara = DBService.Instance.Entities.Characters.Add(newChara);

                newChara.Items.Add(new TCharacterItem { Owner = newChara, ItemID = 1, ItemCount = 10 });
                newChara.Items.Add(new TCharacterItem { Owner = newChara, ItemID = 2, ItemCount = 10 });

                sender.Session.User.Player.Characters.Add(newChara);
                DBService.Instance.Entities.SaveChanges();
            }
            catch(Exception e)
            {
                Log.ErrorFormat("Character Creation Failed In Cause Of {0}", e.Message);
                sender.Session.Response.createChar.Result = Result.Failed;
                sender.Session.Response.createChar.Errormsg = e.Message;

                sender.SendResponse();
                return;
            }

            sender.Session.Response.createChar.Result = Result.Success;
            sender.Session.Response.createChar.Errormsg = "None";
            foreach (var character in sender.Session.User.Player.Characters)
            {
                NCharacterInfo cInfo = new NCharacterInfo();
                cInfo.Id = character.ID;
                cInfo.Name = character.Name;
                cInfo.Type = CharacterType.Player;
                cInfo.Class = (CharacterClass)character.Class;
                cInfo.ConfigId = character.TID;
                sender.Session.Response.createChar.Characters.Add(cInfo);
            }

            sender.SendResponse();
        }
        
        void OnUserEnterGame(NetConnection<NetSession> sender, UserGameEnterRequest request)
        {
            TCharacter dbChara = sender.Session.User.Player.Characters.Where(c => c.ID == request.characterIdx).FirstOrDefault();
            Log.InfoFormat("User Enter Game Request: DB Character ID: {0}, Nick Name: {1}, Map: {2}", request.characterIdx, dbChara.Name, dbChara.MapID);
            var character = CharacterManager.Instance.AddCharacter(dbChara);
            SessionManager.Instance.AddSession(character.Id,sender);

            sender.Session.Response.gameEnter = new UserGameEnterResponse();
            sender.Session.Response.gameEnter.Result = Result.Success;
            sender.Session.Response.gameEnter.Errormsg = "None";
            sender.Session.Response.gameEnter.Character = character.Info;

            sender.Session.Character = character; //Mark the current character
            sender.Session.PostResponser = character;

            sender.SendResponse();
            MapManager.Instance[dbChara.MapID].CharacterEnter(sender, character);
        }
        
        private void OnUserLeaveGame(NetConnection<NetSession> sender, UserGameLeaveRequest request)
        {
            var character = sender.Session.Character;
            Log.InfoFormat("User Leave Game Request: Character ID: {0}, Nick Name: {1}, Map: {2}", character.Id, character.Info.Name, character.Info.mapId);
            SessionManager.Instance.RemoveSession(character.Id);
            CharacterLeave(character);

            sender.Session.Response.gameLeave = new UserGameLeaveResponse();
            sender.Session.Response.gameLeave.Result = Result.Success;
            sender.Session.Response.gameLeave.Errormsg = "None";

            sender.SendResponse();
            sender.Session.Character = null;
        }

        public void CharacterLeave(Character character)
        {
            Log.InfoFormat("CharacterLeaveGame: Character[{0}]:{1}", character.Id, character.Info.Name);
            CharacterManager.Instance.RemoveCharacter(character.Id);
            character.Clear();
            MapManager.Instance[character.Info.mapId].CharacterLeave(character);
        }
    }
}
