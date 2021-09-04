using Managers;
using Network;
using System;
using System.Collections.Generic;
using SkillBridge.Message;
using UnityEngine;

namespace Services
{
    class StoryService : Singleton<StoryService>, IDisposable
    {
        public void Init()
        {
            StoryManager.Instance.Init();
        }

        public StoryService()
        {
            MessageDistributer.Instance.Subscribe<StoryStartResponse>(OnStoryStarted);
            MessageDistributer.Instance.Subscribe<StoryOverResponse>(OnStoryOver);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<StoryStartResponse>(OnStoryStarted);
            MessageDistributer.Instance.Unsubscribe<StoryOverResponse>(OnStoryOver);
        }

        public void SendStartStory(int storyId)
        {
            Debug.Log("SendStartStory: ID:" + storyId);

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.storyStart = new StoryStartRequest();
            message.Request.storyStart.storyId = storyId;
            NetClient.Instance.SendMessage(message);
        }

        private void OnStoryStarted(object sender, StoryStartResponse response)
        {
            Debug.LogFormat("SendStoryStart: ID:{0} Map:{1}", response.storyId, response.instanceId);
            StoryManager.Instance.OnStoryStarted(response.storyId, response.instanceId);
        }

        public void SendStoryOver(int storyId, int instanceId)
        {
            Debug.LogFormat("SendStoryOver: ID:{0} Map:{1}", storyId, instanceId);

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.storyOver = new StoryOverRequest();
            message.Request.storyOver.storyId = storyId;
            message.Request.storyOver.instanceId = instanceId;
            NetClient.Instance.SendMessage(message);
        }

        private void OnStoryOver(object sender, StoryOverResponse response)
        {
            Debug.LogFormat("SendStoryStart: ID:{0}", response.storyId);
            StoryManager.Instance.OnStoryOver(response.storyId);
        }
    }
}
