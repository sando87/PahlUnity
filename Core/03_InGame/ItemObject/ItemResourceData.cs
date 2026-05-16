using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using RPGCharacterAnims.Actions;
using UnityEngine;

namespace PahlBit
{
    public class ItemResourceTable : DatabaseCSV<ItemResourceData> { }

    [System.Serializable]
    public class ItemResourceData : ICSVFormat
    {
        public readonly string ItemID;
        public readonly string DisplayName;
        public readonly string Desc;
        public readonly int LevelLimit;

        public readonly string HealthUp;
        public readonly string HealthRegen;
        public readonly string ManaUp;
        public readonly string ManaRegen;
        public readonly string BaseAttackAdd;
        public readonly string PhyAttack;
        public readonly string FireAttack;
        public readonly string IceAttack;
        public readonly string LightningAttack;
        public readonly string DefenceUp;
        public readonly string MoveSpeedUp;
        public readonly string AttackSpeedUp;
        public readonly string CooltimeDown;
        public readonly string ShieldAdd;
        public readonly string ShieldRegen;

        public readonly string CriticalRate;
        public readonly string CriticalAttack;
        public readonly string ProjectileCountUp;
        public readonly string ProjectileSpeedUp;
        public readonly string AttackRangeUp;
        public readonly string SplashRangeUp;
        public readonly string DurationUp;

        public readonly string FireResist;
        public readonly string IceResist;
        public readonly string LightningResist;
        public readonly string PosionResist;

        public int RowIndex { get; set; } // 데이터데이블상에 존재하는 순서
        public long ID { get { return ICSVFormat.ToID(ItemID); } } // 데이터 접근을 위한 id값

        public ItemAssetData AssetData { get; private set; }

        public ParseValue _HealthUp { get; private set; }
        public ParseValue _HealthRegen { get; private set; }
        public ParseValue _ManaUp { get; private set; }
        public ParseValue _ManaRegen { get; private set; }
        public ParseValue _BaseAttackAdd { get; private set; }
        public ParseValue _PhyAttack { get; private set; }
        public ParseValue _FireAttack { get; private set; }
        public ParseValue _IceAttack { get; private set; }
        public ParseValue _LightningAttack { get; private set; }
        public ParseValue _DefenceUp { get; private set; }
        public ParseValue _MoveSpeedUp { get; private set; }
        public ParseValue _AttackSpeedUp { get; private set; }
        public ParseValue _CooltimeDown { get; private set; }
        public ParseValue _ShieldAdd { get; private set; }
        public ParseValue _ShieldRegen { get; private set; }
        public ParseValue _CriticalRate { get; private set; }
        public ParseValue _CriticalAttack { get; private set; }
        public ParseValue _ProjectileCountUp { get; private set; }
        public ParseValue _ProjectileSpeedUp { get; private set; }
        public ParseValue _AttackRangeUp { get; private set; }
        public ParseValue _SplashRangeUp { get; private set; }
        public ParseValue _DurationUp { get; private set; }
        public ParseValue _FireResist { get; private set; }
        public ParseValue _IceResist { get; private set; }
        public ParseValue _LightningResist { get; private set; }
        public ParseValue _PosionResist { get; private set; }

        void ICSVFormat.OnLoad()
        {
            AssetData = Resources.Load<ItemAssetData>("ScriptableObjects/" + ItemID);

            _HealthUp = ParseValue.Parse(HealthUp);
            _HealthRegen = ParseValue.Parse(HealthRegen);
            _ManaUp = ParseValue.Parse(ManaUp);
            _ManaRegen = ParseValue.Parse(ManaRegen);
            _BaseAttackAdd = ParseValue.Parse(BaseAttackAdd);
            _PhyAttack = ParseValue.Parse(PhyAttack);
            _FireAttack = ParseValue.Parse(FireAttack);
            _IceAttack = ParseValue.Parse(IceAttack);
            _LightningAttack = ParseValue.Parse(LightningAttack);
            _DefenceUp = ParseValue.Parse(DefenceUp);
            _MoveSpeedUp = ParseValue.Parse(MoveSpeedUp);
            _AttackSpeedUp = ParseValue.Parse(AttackSpeedUp);
            _CooltimeDown = ParseValue.Parse(CooltimeDown);
            _ShieldAdd = ParseValue.Parse(ShieldAdd);
            _ShieldRegen = ParseValue.Parse(ShieldRegen);
            _CriticalRate = ParseValue.Parse(CriticalRate);
            _CriticalAttack = ParseValue.Parse(CriticalAttack);
            _ProjectileCountUp = ParseValue.Parse(ProjectileCountUp);
            _ProjectileSpeedUp = ParseValue.Parse(ProjectileSpeedUp);
            _AttackRangeUp = ParseValue.Parse(AttackRangeUp);
            _SplashRangeUp = ParseValue.Parse(SplashRangeUp);
            _DurationUp = ParseValue.Parse(DurationUp);
            _FireResist = ParseValue.Parse(FireResist);
            _IceResist = ParseValue.Parse(IceResist);
            _LightningResist = ParseValue.Parse(LightningResist);
            _PosionResist = ParseValue.Parse(PosionResist);
        }
    }

}