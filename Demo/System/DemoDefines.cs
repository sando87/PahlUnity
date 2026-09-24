using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

namespace PahlUnity.Demo
{
    public static class SystemConstant
    {
    }

    public static class AnimStateNameHash
    {
        public static readonly int Idle = Animator.StringToHash("Idle");
        public static readonly int Run = Animator.StringToHash("Run");
        public static readonly int Jump = Animator.StringToHash("Jump");
        public static readonly int Attack = Animator.StringToHash("Attack");
        public static readonly int Hit = Animator.StringToHash("Hit");
        public static readonly int Death = Animator.StringToHash("Death");
        public static readonly int Dash = Animator.StringToHash("Dash");
    }

    public static class InputActionNameHash
    {
        public static readonly int Navigate = InputManager.GetInputActionNameHash("Navigate");
        public static readonly int Submit = InputManager.GetInputActionNameHash("Submit");
        public static readonly int Cancel = InputManager.GetInputActionNameHash("Cancel");
        public static readonly int Move = InputManager.GetInputActionNameHash("Move");
        public static readonly int Attack = InputManager.GetInputActionNameHash("Attack");
        public static readonly int Dash = InputManager.GetInputActionNameHash("Dash");
        public static readonly int Interact = InputManager.GetInputActionNameHash("Interact");
        public static readonly int Jump = InputManager.GetInputActionNameHash("Jump");

        // public static readonly int UIMove = InputManager.GetInputActionNameHash("UIMove");
        // public static readonly int UIBack = InputManager.GetInputActionNameHash("UIBack");
        // public static readonly int Move = InputManager.GetInputActionNameHash("Move");
        // public static readonly int Dash = InputManager.GetInputActionNameHash("Dash");

        public static readonly int SkillSlotA = InputManager.GetInputActionNameHash("SkillSlotA");
        public static readonly int SkillSlotB = InputManager.GetInputActionNameHash("SkillSlotB");
        public static readonly int SkillSlotC = InputManager.GetInputActionNameHash("SkillSlotC");
        public static readonly int SkillSlotD = InputManager.GetInputActionNameHash("SkillSlotD");

        // public static readonly int PotionA = InputManager.GetInputActionNameHash("PotionA");
        // public static readonly int PotionB = InputManager.GetInputActionNameHash("PotionB");

        // public static readonly int ShowPopupStats = InputManager.GetInputActionNameHash("ShowPopupStats");
        // public static readonly int ShowPopupInven = InputManager.GetInputActionNameHash("ShowPopupInven");
        // public static readonly int ShowPopupSkill = InputManager.GetInputActionNameHash("ShowPopupSkill");
    }

    public static class AnimatorParams
    {
        public static readonly int AttackSpeed = Animator.StringToHash("AttackSpeed");
        public static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
        public static readonly int DoNextCombo = Animator.StringToHash("DoNextCombo");
        public static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
        public static readonly int IsMoving = Animator.StringToHash("IsMoving");
        public static readonly int IsWallAttached = Animator.StringToHash("IsWallAttached");
        public static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
        public static readonly int LockNormal = Animator.StringToHash("LockNormal");
    }

    public static class SceneType
    {
        public static readonly string LogoScene = "00_CompanyLogo";
        public static readonly string Loading = "01_GameLoading";
        public static readonly string MainTitle = "02_MainTitle";
        public static readonly string InGame = "03_InGame";
    }

    public class LayerID
    {
        public static readonly int Player = LayerMask.NameToLayer("Player");
        public static readonly int Enemy = LayerMask.NameToLayer("Enemy");
        public static readonly int Props = LayerMask.NameToLayer("Props");
        public static readonly int Terrain = LayerMask.NameToLayer("Terrain");
        public static readonly int PlayerAttack = LayerMask.NameToLayer("PlayerAttack");
        public static readonly int EnemyAttack = LayerMask.NameToLayer("EnemyAttack");
        public static readonly int ThinPlatform = LayerMask.NameToLayer("ThinPlatform");
        public static readonly int StandableOnThin = LayerMask.NameToLayer("StandableOnThin");
    }

    [System.Serializable]
    public class PlayerData
    {
        public PlayerStatData PlayerStat = new PlayerStatData();
        public Dictionary<long, ItemSaveData> Items = new Dictionary<long, ItemSaveData>();
        public Dictionary<long, SkillSaveData> Skills = new Dictionary<long, SkillSaveData>();
    }

    public enum EquipSlotType
    {
        None,
        Weapon,
        Accessory,
    }

    [System.Serializable]
    public class InGamePlayingData : SaveDataBase
    {
        public Dictionary<long, PlayerData> Characters = new Dictionary<long, PlayerData>();
    }

    public struct DamageInfo : IDamageInfo
    {
        float IDamageInfo.Value => Value;

        public float Value;
        public bool IsPowerAttack;

        public DamageInfo(float _val, bool _powerAttack = false)
        {
            Value = _val;
            IsPowerAttack = _powerAttack;
        }
    }

    public struct SaveUserPlayData : IEventParam
    {
        public readonly bool ImmediateSave;
        public SaveUserPlayData(bool immediateSave) => ImmediateSave = immediateSave;
    }
    public struct SaveUserSettingData : IEventParam
    {
    }
}