using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace PahlUnity
{
    public class Consts
    {
        public const int PointByLevelup = 5;
        public const int SkillPointByLevelup = 1;
    }

    public enum SceneType
    {
        None = -1,
        Main = 0,
        Level1 = 1,
        Level2 = 2,
        Level3 = 3,
        Level4 = 4,
        Level5Boss = 5,
        Level6 = 6,
    }

    public partial struct AnimStateNameHash
    {
        public int mHashValue;
        public AnimStateNameHash(int value)
        {
            mHashValue = value;
        }

        public static implicit operator int(AnimStateNameHash info) => info.mHashValue;
        public static implicit operator AnimStateNameHash(int val) => new AnimStateNameHash(val);

        public static int StringToHash(string stateName) { return Animator.StringToHash(stateName); }

        public static readonly int ExitDummy = Animator.StringToHash("ExitDummy");

        public static readonly int Idle = Animator.StringToHash("Idle");
        public static readonly int Run = Animator.StringToHash("Run");
        public static readonly int Jump = Animator.StringToHash("Jump");
        public static readonly int Attack = Animator.StringToHash("Attack");
        public static readonly int Hit = Animator.StringToHash("Hit");
        public static readonly int Hert = Animator.StringToHash("Hert");
        public static readonly int WakeUp = Animator.StringToHash("WakeUp");
        public static readonly int Damaged = Animator.StringToHash("Damaged");
        public static readonly int Death = Animator.StringToHash("Death");
        public static readonly int Respawn = Animator.StringToHash("Respawn");

        public static readonly int BossAttackA = Animator.StringToHash("BossAttackA");
        public static readonly int BossAttackB = Animator.StringToHash("BossAttackB");
        public static readonly int Frozen = Animator.StringToHash("Frozen");

        public static readonly int Skill = Animator.StringToHash("Skill");
        public static readonly int Skill1 = Animator.StringToHash("Skill1");
        public static readonly int Skill2 = Animator.StringToHash("Skill2");
        public static readonly int Dash = Animator.StringToHash("Dash");
        public static readonly int HitFlying = Animator.StringToHash("HitFlying");
        public static readonly int HitStrong = Animator.StringToHash("HitStrong");

        public static readonly int MeleeA = Animator.StringToHash("MeleeA");
        public static readonly int MeleeB = Animator.StringToHash("MeleeB");
        public static readonly int MeleeC = Animator.StringToHash("MeleeC");
        public static readonly int MeleeD = Animator.StringToHash("MeleeD");

        public static readonly int UpperIdle = Animator.StringToHash("UpperIdle");
        public static readonly int UpperAttack = Animator.StringToHash("UpperAttack");

        // Anims For Enemy
        public static readonly int Sleep = Animator.StringToHash("Sleep");
        public static readonly int Fly = Animator.StringToHash("Fly");

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

    public class MyLayerMask
    {
        public static readonly int Ground = 1 << LayerID.Terrain | 1 << LayerID.ThinPlatform;
    }
    public class StringHashes
    {
        public static readonly int ColorBurn = "ColorBurn".GetHashCode();
        public static readonly int ColorFreez = "ColorFreez".GetHashCode();
    }

    public class PlayerUnitInputType
    {
        public string mValue;
        public PlayerUnitInputType(string value) { mValue = value; }

        public static implicit operator string(PlayerUnitInputType info) => info.mValue;
        public static implicit operator PlayerUnitInputType(string val) => new PlayerUnitInputType(val);

        public static readonly string SkillSlotA = "SkillSlotA";
        public static readonly string SkillSlotB = "SkillSlotB";
        public static readonly string SkillSlotC = "SkillSlotC";
        public static readonly string SkillSlotD = "SkillSlotD";
        public static readonly string UIMove = "UIMove";
        public static readonly string UIBack = "UIBack";
        public static readonly string Move = "Move";
        public static readonly string Jump = "Jump";
        public static readonly string Dash = "Dash";
        public static readonly string PotionA = "PotionA";
        public static readonly string PotionB = "PotionB";
        public static readonly string None = "None";
        public static readonly string ShowPopupStats = "ShowPopupStats";
        public static readonly string ShowPopupInven = "ShowPopupInven";
        public static readonly string ShowPopupSkill = "ShowPopupSkill";
    }


}