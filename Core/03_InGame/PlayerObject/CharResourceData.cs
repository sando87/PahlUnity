using UnityEngine;

namespace PahlUnity
{
    public class CharResourceTable : DatabaseCSV<CharResourceData> { }

    [System.Serializable]
    public class CharResourceData : ICSVFormat
    {
        public readonly string CharacterID;
        public readonly string DisplayName;
        public readonly string Desc;

        public readonly string Health;
        public readonly string Attack;
        public readonly string Defence;
        public readonly string Mana;

        public int RowIndex { get; set; } // 데이터데이블상에 존재하는 순서
        public long ID { get { return ICSVFormat.ToID(CharacterID); } } // 데이터 접근을 위한 id값

        public ParseValue _Health { get; private set; }
        public ParseValue _Attack { get; private set; }
        public ParseValue _Defence { get; private set; }
        public ParseValue _Mana { get; private set; }

        void ICSVFormat.OnLoad()
        {
            _Health = ParseValue.Parse(Health);
            _Attack = ParseValue.Parse(Attack);
            _Defence = ParseValue.Parse(Defence);
            _Mana = ParseValue.Parse(Mana);
        }
    }

}