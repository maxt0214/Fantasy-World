using Common;
using GameServer.Core;
using GameServer.Entities;
using GameServer.Models;
using Network;
using System;
using System.Collections.Generic;

namespace GameServer.Managers
{
    class StoryManager : Singleton<StoryManager>
    {
        public const int MaxInstance = 100;

        public class StoryMap
        {
            public Queue<int> InstanceIdxes = new Queue<int>();
            public Story[] Stories = new Story[MaxInstance];
        }

        private Dictionary<int, StoryMap> storyMaps = new Dictionary<int, StoryMap>();

        public void Init()
        {
            foreach (var story in DataManager.Instance.Stories)
            {
                StoryMap map = new StoryMap();
                for (int i = 0; i < MaxInstance; i++)
                {
                    map.InstanceIdxes.Enqueue(i);
                }
                storyMaps[story.Value.ID] = map;
            }
        }

        public Story NewStory(int storyId, NetConnection<NetSession> owner)
        {
            var storymap = DataManager.Instance.Stories[storyId].MapId;
            var idx = storyMaps[storyId].InstanceIdxes.Dequeue();
            var map = MapManager.Instance.GetInstance(storymap, idx);

            Story story = new Story(map, storyId, idx, owner);
            storyMaps[storyId].Stories[idx] = story;
            story.PlayerIn();
            return story;
        }

        public void RemoveStory(int storyId, int instanceId)
        {
            var storyMap = storyMaps[storyId];
            storyMap.InstanceIdxes.Enqueue(instanceId);
            storyMap.Stories[instanceId] = null;
        }

        internal void Update()
        {
            foreach(var storyMap in storyMaps)
            {
                foreach(var story in storyMap.Value.Stories)
                {
                    if(story != null)
                        story.Update();
                }
            }
        }

        public Story GetStory(int storyId, int instanceId)
        {
            return storyMaps[storyId].Stories[instanceId];
        }
    }
}
