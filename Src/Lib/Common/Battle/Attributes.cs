using System;
using System.Collections.Generic;
using Common.Data;
using SkillBridge.Message;

namespace Common.Battle
{
    public class Attributes
    {
        AttributeData Initial = new AttributeData(); //Original From Character Define
        AttributeData Growth = new AttributeData(); //Growth Per Level From Character Define
        AttributeData Base = new AttributeData(); //Initial + Growth
        AttributeData Equip = new AttributeData(); //From All Equips
        AttributeData Static = new AttributeData(); //Base + Euip
        public AttributeData Buff = new AttributeData(); //From All Buffs
        public AttributeData Final = new AttributeData(); //Static + Final

        public NDynamicAttribte DynamicAttri;

        private int Level;

        /// <summary>
        /// Current HP
        /// </summary>
        public float HP { get { return DynamicAttri.Hp; } set { DynamicAttri.Hp = (int)Math.Min(MaxHP,value); } }

        /// <summary>
        /// Current MP
        /// </summary>
        public float MP { get { return DynamicAttri.Mp; } set { DynamicAttri.Mp = (int)Math.Min(MaxMP, value); } }

        /// <summary>
        /// Maximum HP
        /// </summary>
        public float MaxHP { get { return Final.MaxHP; } }

        /// <summary>
        /// Maximum MP
        /// </summary>
        public float MaxMP { get { return Final.MaxMP; } }

        /// <summary>
        /// Strength
        /// </summary>
        public float STR { get { return Final.STR; } }

        /// <summary>
        /// Intelligence
        /// </summary>
        public float INT { get { return Final.INT; } }

        /// <summary>
        /// Dexterity
        /// </summary>
        public float DEX { get { return Final.DEX; } }

        /// <summary>
        /// Attack Damage
        /// </summary>
        public float AD { get { return Final.AD; } }

        /// <summary>
        /// Ability Power
        /// </summary>
        public float AP { get { return Final.AP; } }

        /// <summary>
        /// Defense
        /// </summary>
        public float DEF { get { return Final.DEF; } }

        /// <summary>
        /// Magic Defense
        /// </summary>
        public float MDEF { get { return Final.MDEF; } }

        /// <summary>
        /// Attack Rate
        /// </summary>
        public float SPD { get { return Final.SPD; } }

        /// <summary>
        /// Chance Of Critical Hit
        /// </summary>
        public float CRI { get { return Final.CRI; } }

        public void Init(CharacterDefine charaDef, int level, List<EquipDefine> equips, NDynamicAttribte dynamAttri)
        {
            Level = level;
            SetInitialAttri(charaDef);
            SetGrowthAttri(charaDef);
            SetBaseAttri();
            SetEquipAttri(equips);
            SetStaticAttri();
            SetFinalAttri();

            if (dynamAttri == null)
            {
                DynamicAttri = new NDynamicAttribte();
                HP = MaxHP;
                MP = MaxMP;
            }
            else
            {
                DynamicAttri = dynamAttri;
                HP = DynamicAttri.Hp;
                MP = DynamicAttri.Mp;
            }
        }

        public void Update(int level)
        {
            Level = level;
            SetBaseAttri();
            SetStaticAttri();
            SetFinalAttri();
        }

        public void Update(List<EquipDefine> equips)
        {
            SetEquipAttri(equips);
            SetStaticAttri();
            SetFinalAttri();
        }

        public void Update(List<BuffDefine> buffs)
        {
            SetBuffAttri(buffs);
            SetFinalAttri();
        }

        //Initial Attributes From Character
        private void SetInitialAttri(CharacterDefine charaDef)
        {
            Initial.MaxHP = charaDef.MaxHP;
            Initial.MaxMP = charaDef.MaxMP;
            Initial.STR = charaDef.STR;
            Initial.INT = charaDef.INT;
            Initial.DEX = charaDef.DEX;
            Initial.AD = charaDef.AD;
            Initial.AP = charaDef.AP;
            Initial.DEF = charaDef.DEF;
            Initial.MDEF = charaDef.MDEF;
            Initial.SPD = charaDef.SPD;
            Initial.CRI = charaDef.CRI;
        }

        //Growth Attribute Per Level
        private void SetGrowthAttri(CharacterDefine charaDef)
        {
            Growth.STR = charaDef.GrowthSTR;
            Growth.INT = charaDef.GrowthINT;
            Growth.DEX = charaDef.GrowthDEX;
        }

        //Base = Initial + Growth
        private void SetBaseAttri()
        {
            for(int i = (int)AttributeType.MAXHP; i < (int)AttributeType.CAP; i++)
            {
                if(i >= (int)AttributeType.STR && i <= (int)AttributeType.DEX)
                    Base.Data[i] = Initial.Data[i] + Growth.Data[i] * (Level - 1); 
                else
                    Base.Data[i] = Initial.Data[i];
            }
        }

        //Equipment Attribute
        private void SetEquipAttri(List<EquipDefine> equips)
        {
            Equip.Reset();
            if (equips == null) return;
            foreach(var def in equips)
            {
                Equip.MaxHP += def.MaxHP;
                Equip.MaxMP += def.MaxMP;
                Equip.STR += def.STR;
                Equip.INT += def.INT;
                Equip.DEX += def.DEX;
                Equip.AD += def.AD;
                Equip.AP += def.AP;
                Equip.DEF += def.DEF;
                Equip.MDEF += def.MDEF;
                Equip.SPD += def.SPD;
                Equip.CRI += def.CRI;
            }
        }

        //Static = Base + Equip
        private void SetStaticAttri()
        {
            //First Degree Attributes
            for (int i = (int)AttributeType.STR; i <= (int)AttributeType.DEX; i++)
            {
                Static.Data[i] = Base.Data[i] + Equip.Data[i];
            }

            //Second Degree Attributes
            Static.MaxHP = Static.STR * 10 + Base.MaxHP + Equip.MaxHP;
            Static.MaxMP = Static.INT * 10 + Base.MaxMP + Equip.MaxMP;

            Static.AD = Static.STR * 5 + Base.AD + Equip.AD;
            Static.AP = Static.INT * 5 + Base.AP + Equip.AP;
            Static.DEF = Static.STR * 2 + Static.DEX * 1 + Base.DEF + Equip.DEF;
            Static.MDEF = Static.INT * 2 + Static.DEX * 1 + Base.MDEF + Equip.MDEF;

            Static.SPD = Static.DEX * 0.2f + Base.SPD + Equip.SPD;
            Static.CRI = Static.DEX * 0.0002f + Base.CRI + Equip.CRI;
        }

        public void SetBuffAttri(List<BuffDefine> buffs)
        {
            Buff.Reset();
            foreach(var def in buffs)
            {

            }
        }

        public void SetFinalAttri()
        {
            for (int i = (int)AttributeType.MAXHP; i < (int)AttributeType.CAP; i++)
            {
                Final.Data[i] = Static.Data[i] + Buff.Data[i];
            }
        }
    }
}
