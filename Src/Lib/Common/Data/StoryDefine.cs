﻿using System;
using System.Collections.Generic;

namespace Common.Data
{
    public class StoryDefine
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MapId { get; set; }
        public int LimitTime { get; set; }
        public int PreQuest { get; set; }
        public int Quest { get; set; }
    }
}
