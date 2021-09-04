using Common;
using Network;
using SkillBridge.Message;
using System;

namespace GameServer.Services
{
    class BagService : Singleton<BagService>, IDisposable
    {
        public BagService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<BagSaveRequest>(OnBagSave);
        }

        public void Init()
        {

        }

        public void Dispose()
        {

        }

        private void OnBagSave(NetConnection<NetSession> sender, BagSaveRequest request)
        {
            var character = sender.Session.Character;
            Log.InfoFormat("BagSaveRequest: Character:{0} Unlocked:{1}", character.Id, request.BagInfo.Unlocked);

            if(character != null)
            {
                character.Data.Bag.items = request.BagInfo.Items;
                DBService.Instance.Save();
            }
        }
    }
}
