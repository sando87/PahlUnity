
using UnityEngine;
using UnityEngine.Events;

namespace PahlUnity
{
    public class PlayerGrowth : MonoBehaviour
    {
        private const int PointByLevelup = 5;
        private const int SkillPointByLevelup = 1;

        // 다음 레벨업을 위해 필요한 경험치량이 증가하는 방식
        private const int FirstExpAtLevelOne = 100;
        private const int FirstExpIncrease = 150;
        private const int IncreaseOfExpIncrease = 50;

        private float mFromExp = 0;
        private float mToExp = 0;
        private CharSaveData mCharacterSaveData = null;

        public int CurrentLevel { get; private set; } = 0;
        public int CurrentLevelIdx { get => CurrentLevel - 1; }

        public int RemainPoint { get => mCharacterSaveData.RemainPoint; }
        public int AttackPoint { get => mCharacterSaveData.AttackPoint; }
        public int DefensePoint { get => mCharacterSaveData.DefensePoint; }
        public int HealthPoint { get => mCharacterSaveData.HealthPoint; }
        public int ManaPoint { get => mCharacterSaveData.ManaPoint; }

        public float ExpAtLevelStart => mFromExp;
        public float ExpForNextLevel => mToExp;
        public float ExpDeltaOfCurrentLevel => mToExp - mFromExp;
        public float RemainExp { get { return mToExp - CurrentExp; } }
        public float CurrentExpRate { get { return (CurrentExp - mFromExp) / (float)ExpDeltaOfCurrentLevel; } }
        public float CurrentExp { get; private set; } = 0;

        public UnityEvent OnLevelUp = new UnityEvent();
        public UnityEvent OnStatPointChanged = new UnityEvent();

        void Start()
        {
            // BattleDispatcher battleDispatcher = this.ExGetBase().GetComponentInChildren<BattleDispatcher>();
            // if (battleDispatcher != null)
            // {
            //     battleDispatcher.EventOnKillResult.AddListener((result) =>
            //     {
            //         if (result.IsKilled)
            //         {
            //             EnemyBase enemy = result.Target.ExGetBase().EnemyObj;
            //             if (enemy != null)
            //             {
            //                 float gainedExp = enemy.Spec.ExpOnDeath;
            //                 AddExp(gainedExp);
            //             }
            //         }
            //     });
            // }
        }

        public void Init(CharSaveData characterSaveData)
        {
            mCharacterSaveData = characterSaveData;
            CurrentExp = mCharacterSaveData.CurrentExp;
            CurrentLevel = GetLevelFromAccExp(mCharacterSaveData.CurrentExp);

            mFromExp = GetAccExpForNextLevel(CurrentLevel - 1);
            mToExp = GetAccExpForNextLevel(CurrentLevel);
        }

        public void AddExp(float exp)
        {
            CurrentExp += exp;
            mCharacterSaveData.CurrentExp = CurrentExp;
            GameSystem.RequestSave();

            while (CurrentExp.ToInt() >= mToExp.ToInt())
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            CurrentLevel += 1;
            mCharacterSaveData.RemainPoint += PointByLevelup;
            mCharacterSaveData.RemainSkillPoint += SkillPointByLevelup;
            mFromExp = mToExp;
            mToExp = GetAccExpForNextLevel(CurrentLevel);

            OnLevelUp?.Invoke();

            GameSystem.DoSave_UserSaveData();
        }

        public void AddAttackPoint()
        {
            if (mCharacterSaveData.RemainPoint > 0)
            {
                mCharacterSaveData.AttackPoint += 1;
                mCharacterSaveData.RemainPoint -= 1;
                OnStatPointChanged?.Invoke();
                GameSystem.DoSave_UserSaveData();
            }
        }
        public void AddDefensePoint()
        {
            if (mCharacterSaveData.RemainPoint > 0)
            {
                mCharacterSaveData.DefensePoint += 1;
                mCharacterSaveData.RemainPoint -= 1;
                OnStatPointChanged?.Invoke();
                GameSystem.DoSave_UserSaveData();
            }
        }
        public void AddHealthPoint()
        {
            if (mCharacterSaveData.RemainPoint > 0)
            {
                mCharacterSaveData.HealthPoint += 1;
                mCharacterSaveData.RemainPoint -= 1;
                OnStatPointChanged?.Invoke();
                GameSystem.DoSave_UserSaveData();
            }
        }
        public void AddManaPoint()
        {
            if (mCharacterSaveData.RemainPoint > 0)
            {
                mCharacterSaveData.ManaPoint += 1;
                mCharacterSaveData.RemainPoint -= 1;
                OnStatPointChanged?.Invoke();
                GameSystem.DoSave_UserSaveData();
            }
        }

        public static int GetAccExpForNextLevel(int curLevel)
        {
            if (curLevel <= 0)
                return 0;

            int levelDown = Mathf.Max(0, curLevel - 1);
            int levelDownDown = Mathf.Max(0, curLevel - 2);
            return FirstExpAtLevelOne
            + levelDown * FirstExpIncrease
            + levelDown * levelDownDown / 2 * IncreaseOfExpIncrease;
        }
        public static int GetLevelFromAccExp(float accumulatedExp)
        {
            if (accumulatedExp < FirstExpAtLevelOne)
                return 1;

            float A = IncreaseOfExpIncrease / 2f;
            float B = FirstExpIncrease - (3f * IncreaseOfExpIncrease / 2f);
            float C = FirstExpAtLevelOne - FirstExpIncrease + IncreaseOfExpIncrease - accumulatedExp;

            float discriminant = B * B - 4f * A * C;

            if (discriminant < 0f)
                return 0; // 예외 처리

            float L = (-B + Mathf.Sqrt(discriminant)) / (2f * A);

            return Mathf.FloorToInt(L) + 1;
        }

    }

}