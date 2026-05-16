using UnityEngine;
using UnityEngine.Events;

namespace PahlUnity
{
    public struct DamagedResultInfo
    {
        public int OriDamage;
        public int TotalDamage;
        public int ValidDamage;

        public int BeforeHealth;
        public int AfterHealth;
        public int MaxHealth;

        public int DeltaHealth => BeforeHealth - AfterHealth;
        public float CurrentHealthRate => AfterHealth / (float)MaxHealth;
    }
}
