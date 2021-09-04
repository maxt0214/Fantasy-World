using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Battle
{
    public enum AttributeType
    {
        NONE = -1,

        MAXHP = 0,

        MAXMP = 1,

        STR = 2,

        INT = 3,

        DEX = 4,

        AD = 5,

        AP = 6,

        DEF = 7,

        MDEF = 8,

        SPD = 9,

        CRI = 10,

        CAP
    }

    public enum TargetType
    {
        None = 0,

        Self = 1,

        Single = 2,

        Area = 3,
    }

    public enum BuffEffect
    {
        None = 0,

        Stun = 1,

        Invincible = 2,
    }

    public enum SkillType
    {
        All = -1,

        Normal = 1,

        Skill = 2,
    }

    public enum DmgType
    {
        Physical = 0,

        Magical = 1,
    }

    public enum TriggerType
    {
        None = 0,

        SkillHit = 1,

        SkillCast = 2
    }

    public enum CreatureState
    {
        None,
        Idle,
        InBattle
    }
}
