using UnityEngine;

namespace PahlUnity
{
    [System.Serializable]
    public class EnemyResourceData : ITableRecord
    {
        public readonly string EnemyID;
        public readonly string DisplayName;
        public readonly string Desc;

        public readonly string Health;
        public readonly string Mana;
        public readonly string Shield;
        public readonly string Attack;
        public readonly string Defence;
        public readonly string MoveSpeed;
        public readonly string AttackSpeed;
        public readonly string Cooltime;
        public readonly string DetectRange;
        public readonly string AttackRange;
        public readonly string ItemDrop;
        public readonly string GoldOnDeath;
        public readonly string ExpOnDeath;
        public readonly string HitChance;

        public int RowIndex { get; set; } // 데이터데이블상에 존재하는 순서
        public long ID { get { return ITableRecord.ToID(EnemyID); } } // 데이터 접근을 위한 id값

        public ParseValue _Health { get; private set; }
        public ParseValue _Mana { get; private set; }
        public ParseValue _Shield { get; private set; }
        public ParseValue _Attack { get; private set; }
        public ParseValue _Defence { get; private set; }
        public ParseValue _MoveSpeed { get; private set; }
        public ParseValue _AttackSpeed { get; private set; }
        public ParseValue _Cooltime { get; private set; }
        public ParseValue _DetectRange { get; private set; }
        public ParseValue _AttackRange { get; private set; }
        public ParseValue _ItemDrop { get; private set; }
        public ParseValue _GoldOnDeath { get; private set; }
        public ParseValue _ExpOnDeath { get; private set; }
        public ParseValue _HitChance { get; private set; }

        void ITableRecord.OnLoad()
        {
            _Health = ParseValue.Parse(Health);
            _Mana = ParseValue.Parse(Mana);
            _Shield = ParseValue.Parse(Shield);
            _Attack = ParseValue.Parse(Attack);
            _Defence = ParseValue.Parse(Defence);
            _MoveSpeed = ParseValue.Parse(MoveSpeed);
            _AttackSpeed = ParseValue.Parse(AttackSpeed);
            _Cooltime = ParseValue.Parse(Cooltime);
            _DetectRange = ParseValue.Parse(DetectRange);
            _AttackRange = ParseValue.Parse(AttackRange);
            _ItemDrop = ParseValue.Parse(ItemDrop);
            _GoldOnDeath = ParseValue.Parse(GoldOnDeath);
            _ExpOnDeath = ParseValue.Parse(ExpOnDeath);
            _HitChance = ParseValue.Parse(HitChance);
        }
    }

}