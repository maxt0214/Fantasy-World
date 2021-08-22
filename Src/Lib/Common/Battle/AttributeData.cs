using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Battle
{
    public class AttributeData
    {
        public float[] Data = new float[(int)AttributeType.CAP];

        /// <summary>
        /// Maximum HP
        /// </summary>
        public float MaxHP { get { return Data[(int)AttributeType.MAXHP]; } set { Data[(int)AttributeType.MAXHP] = value; } }

        /// <summary>
        /// Maximum MP
        /// </summary>
        public float MaxMP { get { return Data[(int)AttributeType.MAXMP]; } set { Data[(int)AttributeType.MAXMP] = value; } }

        /// <summary>
        /// Strength
        /// </summary>
        public float STR { get { return Data[(int)AttributeType.STR]; } set { Data[(int)AttributeType.STR] = value; } }

        /// <summary>
        /// Intelligence
        /// </summary>
        public float INT { get { return Data[(int)AttributeType.INT]; } set { Data[(int)AttributeType.INT] = value; } }

        /// <summary>
        /// Dexterity
        /// </summary>
        public float DEX { get { return Data[(int)AttributeType.DEX]; } set { Data[(int)AttributeType.DEX] = value; } }

        /// <summary>
        /// Attack Damage
        /// </summary>
        public float AD { get { return Data[(int)AttributeType.AD]; } set { Data[(int)AttributeType.AD] = value; } }

        /// <summary>
        /// Ability Power
        /// </summary>
        public float AP { get { return Data[(int)AttributeType.AP]; } set { Data[(int)AttributeType.AP] = value; } }

        /// <summary>
        /// Defense
        /// </summary>
        public float DEF { get { return Data[(int)AttributeType.DEF]; } set { Data[(int)AttributeType.DEF] = value; } }

        /// <summary>
        /// Magic Defense
        /// </summary>
        public float MDEF { get { return Data[(int)AttributeType.MDEF]; } set { Data[(int)AttributeType.MDEF] = value; } }

        /// <summary>
        /// Attack Rate
        /// </summary>
        public float SPD { get { return Data[(int)AttributeType.SPD]; } set { Data[(int)AttributeType.SPD] = value; } }

        /// <summary>
        /// Chance Of Critical Hit
        /// </summary>
        public float CRI { get { return Data[(int)AttributeType.CRI]; } set { Data[(int)AttributeType.CRI] = value; } }

        public void Reset()
        {
            for(int i = 0; i < Data.Length; i++)
            {
                Data[i] = 0f;
            }
        }
    }
}
