using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

namespace PahlUnity
{
    public readonly partial struct AnimStateHash
    {
        public static readonly AnimStateHash Idle = new("Idle");
        public static readonly AnimStateHash Run = new("Run");
        public static readonly AnimStateHash Jump = new("Jump");
        public static readonly AnimStateHash Attack = new("Attack");
        public static readonly AnimStateHash Hert = new("Hert");
        public static readonly AnimStateHash Death = new("Death");
    }

    public readonly partial struct InputActionName
    {
        public static readonly InputActionName Jump = new("Jump");
        public static readonly InputActionName Dash = new("Dash");

        public static readonly InputActionName SkillSlotA = new("SkillSlotA");
        public static readonly InputActionName SkillSlotB = new("SkillSlotB");
        public static readonly InputActionName SkillSlotC = new("SkillSlotC");
        public static readonly InputActionName SkillSlotD = new("SkillSlotD");

        public static readonly InputActionName PotionA = new("PotionA");
        public static readonly InputActionName PotionB = new("PotionB");

        public static readonly InputActionName ShowPopupStats = new("ShowPopupStats");
        public static readonly InputActionName ShowPopupInven = new("ShowPopupInven");
        public static readonly InputActionName ShowPopupSkill = new("ShowPopupSkill");
    }

    namespace Demo
    {
        public static class AnimatorParams
        {
            public static readonly int AttackSpeed = Animator.StringToHash("AttackSpeed");
            public static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
            public static readonly int DoNextCombo = Animator.StringToHash("DoNextCombo");
            public static readonly int StopLoop = Animator.StringToHash("StopLoop");
            public static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
            // public static readonly int IsGround = Animator.StringToHash("IsGround");
            // public static readonly int IsMoving = Animator.StringToHash("IsMoving");
        }

        public static class SceneType
        {
            public static readonly string LogoScene = "LogoScene";
            public static readonly string Loading = "Loading";
            public static readonly string MainTitle = "MainTitle";
            public static readonly string InGame = "InGame";
        }

        public class LayerID
        {
            public static readonly int Terrain = LayerMask.NameToLayer("Terrain");
            public static readonly int Player = LayerMask.NameToLayer("Player");
            public static readonly int Enemy = LayerMask.NameToLayer("Enemy");
            public static readonly int Neutral = LayerMask.NameToLayer("Neutral");
            public static readonly int PlatformPlayer = LayerMask.NameToLayer("PlatformPlayer");
            public static readonly int PlayerObject = LayerMask.NameToLayer("PlayerObject");
            public static readonly int PlatformEnemy = LayerMask.NameToLayer("PlatformEnemy");
            public static readonly int Platform = LayerMask.NameToLayer("Platform");
            public static readonly int ThinPlatform = LayerMask.NameToLayer("ThinPlatform");
            public static readonly int StandableOnThin = LayerMask.NameToLayer("StandableOnThin");
        }

        public static class InteractMaskDemo
        {
            public const InteractMask DetectSignal = (InteractMask)(1 << 6);
            public const InteractMask TriggerSignal = (InteractMask)(1 << 7);
        }

        [System.Serializable]
        public class PlayerSaveData : SaveDataBase
        {
            public int PlayerGold;
        }
    }
}