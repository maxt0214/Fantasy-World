using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Common.Data
{
    public enum QuestType 
    {
        Main,
        Branch
    }

    public enum QuestTarget
    {
        None,
        Kill,
        Item
    }

    public class QuestDefine
    {
        public int ID { get; set; }
    }
}
