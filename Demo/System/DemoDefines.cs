using System.Collections.Generic;
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

    namespace Demo
    {
        public static class InputActionNameHash
        {
            public static readonly int UIMove = InputManager.GetInputActionNameHash("UIMove");
            public static readonly int UIBack = InputManager.GetInputActionNameHash("UIBack");
            public static readonly int Move = InputManager.GetInputActionNameHash("Move");
            public static readonly int Jump = InputManager.GetInputActionNameHash("Jump");
            public static readonly int Dash = InputManager.GetInputActionNameHash("Dash");

            public static readonly int SkillSlotA = InputManager.GetInputActionNameHash("SkillSlotA");
            public static readonly int SkillSlotB = InputManager.GetInputActionNameHash("SkillSlotB");
            public static readonly int SkillSlotC = InputManager.GetInputActionNameHash("SkillSlotC");
            public static readonly int SkillSlotD = InputManager.GetInputActionNameHash("SkillSlotD");

            public static readonly int PotionA = InputManager.GetInputActionNameHash("PotionA");
            public static readonly int PotionB = InputManager.GetInputActionNameHash("PotionB");

            public static readonly int ShowPopupStats = InputManager.GetInputActionNameHash("ShowPopupStats");
            public static readonly int ShowPopupInven = InputManager.GetInputActionNameHash("ShowPopupInven");
            public static readonly int ShowPopupSkill = InputManager.GetInputActionNameHash("ShowPopupSkill");
        }

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
        public class PlayerData
        {
            public PlayerStatData PlayerStat = new PlayerStatData();
            public Dictionary<long, ItemSaveData> Items = new Dictionary<long, ItemSaveData>();
            public Dictionary<long, SkillSaveData> Skills = new Dictionary<long, SkillSaveData>();
        }

        [System.Serializable]
        public class InGamePlayingData : SaveDataBase
        {
            public Dictionary<long, PlayerData> Characters = new Dictionary<long, PlayerData>();
        }
    }
}