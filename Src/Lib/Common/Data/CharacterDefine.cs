using SkillBridge.Message;

namespace Common.Data
{
    public class CharacterDefine
    {
        public int TID { get; set; }
        public string Name { get; set; }
        public CharacterClass Class { get; set; }
        public string Resource { get; set; }
        public string Description { get; set; }
        public string AI { get; set; }
        public int InitLevel { get; set; }
        public float Height { get; set; }
        public int Speed { get; set; }
        public float MaxHP { get; set; }
        public float MaxMP { get; set; }
        public float GrowthSTR { get; set; }
        public float GrowthINT { get; set; }
        public float GrowthDEX { get; set; }
        public float STR { get; set; }
        public float INT { get; set; }
        public float DEX { get; set; }
        public float AD { get; set; }
        public float AP { get; set; }
        public float DEF { get; set; }
        public float MDEF { get; set; }
        public float SPD { get; set; } //Attack Rate
        public float CRI { get; set; }
    }
}
