﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Data
{
    public enum MapType
    {
        Normal = 0,
        Arena = 1,
        Story = 2
    }

    public class MapDefine
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Resource { get; set; }
        public string MiniMap { get; set; }
        public string Music { get; set; }
        public MapType Type { get; set; }
    }
}
