using System;
using System.Collections.Generic;
using Common;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Models;
using Network;
using SkillBridge.Message;

namespace GameServer.Services
{
    class StoryService : Singleton<StoryService>
    {
        public StoryService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<StoryStartRequest>(OnStartStory);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<StoryOverRequest>(OnStoryOver);
        }

        public void Init()
        {
            StoryManager.Instance.Init();
        }

        private void OnStartStory(NetConnection<NetSession> sender, StoryStartRequest request)
        {
            Character chara = sender.Session.Character;
            Log.InfoFormat("OnStartStory: StoryId:{0}", request.storyId);

            var story = StoryManager.Instance.NewStory(request.storyId,sender);

            sender.Session.Response.storyStart = new StoryStartResponse();
            sender.Session.Response.storyStart.storyId = story.StoryId;
            sender.Session.Response.storyStart.instanceId = story.InstanceId;
            sender.Session.Response.storyStart.Result = Result.Success;
            sender.Session.Response.storyStart.Errormsg = "";
            sender.SendResponse();
        }

        private void OnStoryOver(NetConnection<NetSession> sender, StoryOverRequest request)
        {
            Log.InfoFormat("OnStoryOver: StoryId:{0} InstanceId:{1}", request.storyId, request.instanceId);
            var story = StoryManager.Instance.GetStory(request.storyId,request.instanceId);
            if (story == null)
                return;
            story.Clear();

            sender.Session.Response.storyOver = new StoryOverResponse();
            sender.Session.Response.storyOver.storyId = story.StoryId;
            sender.Session.Response.storyOver.Result = Result.Success;
            sender.Session.Response.storyOver.Errormsg = "";
            sender.SendResponse();
        }
    }
}
