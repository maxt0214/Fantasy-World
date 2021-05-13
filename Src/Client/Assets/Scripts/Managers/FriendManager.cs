using System.Collections.Generic;
using SkillBridge.Message;

namespace Managers
{
    class FriendManager : Singleton<FriendManager>
    {
        public List<NFriendInfo> friends = new List<NFriendInfo>();

        public void Init(List<NFriendInfo> friends)
        {
            this.friends = friends;
        }
    }
}
