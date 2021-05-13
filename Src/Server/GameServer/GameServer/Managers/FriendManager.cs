using Common;
using GameServer.Entities;
using System.Collections.Generic;
using SkillBridge.Message;
using System.Linq;
using GameServer.Services;

namespace GameServer.Managers
{
    class FriendManager
    {
        private Character Owner;

        List<NFriendInfo> friends = new List<NFriendInfo>();
        bool friendChanged = false;

        public FriendManager(Character character)
        {
            Owner = character;
            InitFriends();
        }

        private void InitFriends()
        {
            friends.Clear();
            foreach(var friend in Owner.Data.Friends)
            {
                friends.Add(GetFriendInfo(friend));
            }
        }

        public void GetFriendInfos(List<NFriendInfo> friends)
        {
            foreach(var friend in this.friends)
            {
                friends.Add(friend);
            }
        }

        public void AddFriend(Character character)
        {
            TCharacterFriend tf = new TCharacterFriend()
            {
                FriendID = character.Id,
                FriendName = character.Data.Name,
                Class = character.Data.Class,
                Level = character.Data.Level
            };
            Owner.Data.Friends.Add(tf);
            friendChanged = true;
        }

        public bool RemoveFriendByID(int friendRelationId)
        {
            var toRemove = Owner.Data.Friends.FirstOrDefault(f => f.Id == friendRelationId);
            if (toRemove != null)
            {
                DBService.Instance.Entities.CharacterFriends.Remove(toRemove);
            }
            friendChanged = true;
            return true;
        }

        public bool RemoveFriendByFriendID(int friendId)
        {
            var toRemove = Owner.Data.Friends.FirstOrDefault(f => f.FriendID == friendId);
            if(toRemove != null)
            {
                DBService.Instance.Entities.CharacterFriends.Remove(toRemove);
            }
            friendChanged = true;
            return true;
        }

        public NFriendInfo GetFriendInfo(TCharacterFriend friend)
        {
            NFriendInfo friendInfo = new NFriendInfo();
            var character = CharacterManager.Instance.GetCharacter(friend.FriendID);
            friendInfo.friendInfo = new NCharacterInfo();
            friendInfo.Id = friend.Id;
            if(character == null)
            {
                friendInfo.friendInfo.Id = friend.FriendID;
                friendInfo.friendInfo.Name = friend.FriendName;
                friendInfo.friendInfo.Class = (CharacterClass)friend.Class;
                friendInfo.friendInfo.Level = friend.Level;
                friendInfo.Status = 0;
            } else
            {
                friendInfo.friendInfo = character.GetBasicInfo();
                friendInfo.friendInfo.Name = character.Info.Name;
                friendInfo.friendInfo.Class = character.Info.Class;
                friendInfo.friendInfo.Level = character.Info.Level;

                if (friend.Level != character.Info.Level)
                    friend.Level = character.Info.Level;

                character.FriendManager.UpdateFriendInfo(Owner.Info,1);
                friendInfo.Status = 1;
            }
            Log.InfoFormat("Owner[{0}]:{1} GetFriendInfo: Friend[{2}]:{3} Status:{4}", Owner.Id, Owner.Info.Name, friendInfo.Id, friendInfo.friendInfo.Name, friendInfo.Status);
            return friendInfo;
        }

        public NFriendInfo GetFriendInfo(int friendId)
        {
            foreach(var friend in friends)
            {
                if (friend.Id == friendId)
                    return friend;
            }
            return null;
        }

        public void UpdateFriendInfo(NCharacterInfo info, int status)
        {
            foreach(var friend in friends)
            {
                if(friend.friendInfo.Id == info.Id)
                {
                    friend.Status = status;
                    break;
                }
            }
            friendChanged = true;
        }

        public void OfflineNotify()
        {
            foreach(var friendInfo in friends)
            {
                var friend = CharacterManager.Instance.GetCharacter(friendInfo.friendInfo.Id);
                if(friend != null)
                {
                    friend.FriendManager.UpdateFriendInfo(Owner.Info,0);
                }
            }
        }

        public void PostProcess(NetMessageResponse response)
        {
            if (friendChanged)
            {
                InitFriends();
                if (response.friendList == null)
                {
                    response.friendList = new FriendListResponse();
                    response.friendList.Friends.AddRange(friends);
                }
                friendChanged = false;
            }
        }
    }
}
