using Newtonsoft.Json;
using UnityEngine;

namespace PahlUnity.Demo
{
    [System.Serializable]
    public class PlayerSaveData
    {
        public float CurrentExp;
        public int RemainPoint;
        public int RemainSkillPoint;
        public int HealthPoint;
        public int ManaPoint;
        public int AttackPoint;
        public int DefensePoint;
    }
}