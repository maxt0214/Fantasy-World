using SkillBridge.Message;
using Common.Battle;

namespace Common.Data
{
    public class BuffDefine
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public TargetType TargetType { get; set; }
        public BuffEffect Effect { get; set; }
        public TriggerType Trigger { get; set; }
        public int CD { get; set; }
        public float Duration { get; set; }
        public float Interval { get; set; }
        //Base Stat For calculating dmg
        public float AD { get; set; }
        public float AP { get; set; }
        //Factors over character attributes for additional dmg
        public float ADFactor { get; set; }
        public float APFactor { get; set; }
        //Effect on character attributes
        public float DEFRatio { get; set; }
    }
}
