using SkillBridge.Message;
using Common.Battle;
using System.Collections.Generic;

namespace Common.Data
{
    public class SkillDefine
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string SkillAnim { get; set; }
        public SkillType SkillType { get; set; }
        public int Level { get; set; }
        public TargetType TargetType { get; set; }
        public int NumTarget { get; set; }
        public int Range { get; set; }
        public float CastTime { get; set; }
        public float CD { get; set; }
        public int MPCost { get; set; }
        public int ContinousMPCost { get; set; }
        public int HPCost { get; set; }
        public int ContinousHPCost { get; set; }
        public bool IfBullet { get; set; }
        public float BulletSpd { get; set; }
        public string BulletRes { get; set; }
        public int AOERange { get; set; }
        public float Duration { get; set; }
        public float EffectInterval { get; set; }
        public List<float> HitTimes { get; set; }
        public List<int> Buff { get; set; }
        public float DmgMultiplier { get; set; }
        public float ContinousDmg { get; set; }
        public DmgType DmgType { get; set; }
    }
}
