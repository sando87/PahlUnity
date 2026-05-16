
using UnityEngine;
using UnityEngine.Events;

namespace PahlUnity
{
    public class Experience : MonoBehaviour
    {
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

        public void Init(int characterID)
        {
            UserSaveData userSaveData = null; // SaveFileManager<UserSaveData>.Load();
            mCharacterSaveData = userSaveData.Characters[characterID].Stats;
            CurrentExp = mCharacterSaveData.CurrentExp;
            CurrentLevel = GameSystem.GetLevelFromAccExp(mCharacterSaveData.CurrentExp);

            mFromExp = GameSystem.GetAccExpForNextLevel(CurrentLevel - 1);
            mToExp = GameSystem.GetAccExpForNextLevel(CurrentLevel);
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
            mCharacterSaveData.RemainPoint += Consts.PointByLevelup;
            mCharacterSaveData.RemainSkillPoint += Consts.SkillPointByLevelup;
            mFromExp = mToExp;
            mToExp = GameSystem.GetAccExpForNextLevel(CurrentLevel);

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


    }

}