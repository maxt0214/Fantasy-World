using Common;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class HelloWorldService : Singleton<HelloWorldService>
    {
        public void Init()
        {

        }

        public void Start()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FirstTestRequest>(OnFirstTestRequest);
        }

        public void Stop()
        {

        }

        void OnFirstTestRequest(NetConnection<NetSession> sender, FirstTestRequest request)
        {
            Log.InfoFormat("UserFirstTestRequest: Helloworld:{0}", request.helloWorld);
        }
    }
}
