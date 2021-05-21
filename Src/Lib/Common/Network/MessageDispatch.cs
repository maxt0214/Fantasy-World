//WARNING: DON'T EDIT THIS FILE!!!
using Common;

namespace Network
{
    public class MessageDispatch<T> : Singleton<MessageDispatch<T>>
    {
        //Responses are handled on clients
        public void Dispatch(T sender, SkillBridge.Message.NetMessageResponse message)
        {
            if (message.userRegister != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.userRegister); }
            if (message.userLogin != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.userLogin); }
            if (message.createChar != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.createChar); }
            if (message.gameEnter != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.gameEnter); }
            if (message.gameLeave != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.gameLeave); }
            if (message.mapCharacterEnter != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.mapCharacterEnter); }
            if (message.mapCharacterLeave != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.mapCharacterLeave); }
            if (message.mapEntitySync != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.mapEntitySync); }
            if (message.itemPurchase != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.itemPurchase); }
            if (message.statusNotify != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.statusNotify); }
            if (message.ItemEquip != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.ItemEquip); }
            if (message.questAccept != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.questAccept); }
            if (message.questSubmit != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.questSubmit); }
            if (message.friendAddReq != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.friendAddReq); }
            if (message.friendAddRes != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.friendAddRes); }
            if (message.friendList != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.friendList); }
            if (message.friendRemove != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.friendRemove); }
            if (message.teamInviteReq != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.teamInviteReq); }
            if (message.teamInviteRes != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.teamInviteRes); }
            if (message.teamInfo != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.teamInfo); }
            if (message.teamLeave != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.teamLeave); }
            if (message.guildCreation != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.guildCreation); }
            if (message.guildJoinReq != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.guildJoinReq); }
            if (message.guildJoinRes != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.guildJoinRes); }
            if (message.guildInfo != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.guildInfo); }
            if (message.guildLeave != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.guildLeave); }
            if (message.guildList != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.guildList); }
            if (message.guildAdmin != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.guildAdmin); }
        }

        //Requests are handled on the server
        public void Dispatch(T sender, SkillBridge.Message.NetMessageRequest message)
        {
            if (message.userRegister != null) { MessageDistributer<T>.Instance.RaiseEvent(sender,message.userRegister); }
            if (message.userLogin != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.userLogin); }
            if (message.createChar != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.createChar); }
            if (message.gameEnter != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.gameEnter); }
            if (message.gameLeave != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.gameLeave); }
            if (message.mapCharacterEnter != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.mapCharacterEnter); }
            if (message.mapEntitySync != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.mapEntitySync); }
            if (message.mapTeleport != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.mapTeleport); }
            if (message.itemPurchase != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.itemPurchase); }
            if (message.ItemEquip != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.ItemEquip); }
            if (message.questAccept != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.questAccept); }
            if (message.questSubmit != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.questSubmit); }
            if (message.friendAddReq != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.friendAddReq); }
            if (message.friendAddRes != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.friendAddRes); }
            if (message.friendList != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.friendList); }
            if (message.friendRemove != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.friendRemove); }
            if (message.teamInviteReq != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.teamInviteReq); }
            if (message.teamInviteRes != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.teamInviteRes); }
            if (message.teamInfo != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.teamInfo); }
            if (message.teamLeave != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.teamLeave); }
            if (message.guildCreation != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.guildCreation); }
            if (message.guildJoinReq != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.guildJoinReq); }
            if (message.guildJoinRes != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.guildJoinRes); }
            if (message.guildInfo != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.guildInfo); }
            if (message.guildLeave != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.guildLeave); }
            if (message.guildList != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.guildList); }
            if (message.guildAdmin != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.guildAdmin); }
        }
    }
}