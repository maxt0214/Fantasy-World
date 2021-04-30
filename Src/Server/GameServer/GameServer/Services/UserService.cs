﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Network;
using SkillBridge.Message;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Models;

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
                    message.Response.userLogin.Userinfo.Id = (int)user.ID;
                    message.Response.userLogin.Userinfo.Player = new NPlayerInfo();
                    message.Response.userLogin.Userinfo.Player.Id = user.Player.ID;
                    foreach (var character in user.Player.Characters) 
                    {
                        NCharacterInfo cInfo = new NCharacterInfo();
                        cInfo.Id = character.ID;
                        cInfo.Name = character.Name;
                        cInfo.Type = CharacterType.Player;
                        cInfo.Class = (CharacterClass)character.Class;
                        cInfo.Tid = character.ID;
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
                Gold = 10000,
                Equips = new byte[28],
            };

            byte[] data;
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
                cInfo.Type = CharacterType.Player;
                cInfo.Class = (CharacterClass)character.Class;
                cInfo.Tid = character.ID;
                message.Response.createChar.Characters.Add(cInfo);
            }

            data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
        }
        
        void OnUserEnterGame(NetConnection<NetSession> sender, UserGameEnterRequest request)
        {
            TCharacter dbChara = sender.Session.User.Player.Characters.Where(c => c.ID == request.characterIdx).FirstOrDefault();
            Log.InfoFormat("User Enter Game Request: DB Character ID: {0}, Nick Name: {1}, Map: {2}", request.characterIdx, dbChara.Name, dbChara.MapID);
            var character = CharacterManager.Instance.AddCharacter(dbChara);

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.gameEnter = new UserGameEnterResponse();
            message.Response.gameEnter.Result = Result.Success;
            message.Response.gameEnter.Errormsg = "None";
            message.Response.gameEnter.Character = character.Info;

            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
            sender.Session.Character = character; //Mark the current character
            MapManager.Instance[dbChara.MapID].CharacterEnter(sender, character);
        }
        
        private void OnUserLeaveGame(NetConnection<NetSession> sender, UserGameLeaveRequest request)
        {
            var character = sender.Session.Character;
            Log.InfoFormat("User Leave Game Request: Character ID: {0}, Nick Name: {1}, Map: {2}", character.Id, character.Info.Name, character.Info.mapId);

            CharacterLeave(character);
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.gameLeave = new UserGameLeaveResponse();
            message.Response.gameLeave.Result = Result.Success;
            message.Response.gameLeave.Errormsg = "None";

            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
        }

        public void CharacterLeave(Character character)
        {
            CharacterManager.Instance.RemoveCharacter(character.Id);
            MapManager.Instance[character.Info.mapId].CharacterLeave(character);
        }
    }
}
