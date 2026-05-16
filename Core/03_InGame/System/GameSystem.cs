using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

namespace PahlUnity
{
    public static class GameSystem
    {
        static float SaveRequestedTime = 0;

        static public void DoSave_UserSaveData()
        {
            // UserSaveData saveData = SaveFileManager<UserSaveData>.Load();
            // SaveFileManager<UserSaveData>.Save(saveData);
        }

        // 바로 세이브 하지 않고 일정 시간 동안 다시 RequestSave() 호출이 없으면 그때 세이브 하는 방식
        // 골드나 경험치 같이 자주 변경되는 데이터를 위한 세이브 방식
        static public void RequestSave()
        {
            bool isAlreadyRequested = SaveRequestedTime != 0;
            SaveRequestedTime = Time.time;
            if (!isAlreadyRequested)
            {
                WaitSaveUntilBreak().Forget();
            }
        }
        static async UniTask WaitSaveUntilBreak()
        {
            while (Time.time - SaveRequestedTime < 3.0f)
            {
                await UniTask.DelayFrame(3);
            }
            SaveRequestedTime = 0;
            DoSave_UserSaveData();
        }

        // 다음 레벨업을 위해 필요한 경험치량이 증가하는 방식
        static int FirstExpAtLevelOne = 100;
        static int FirstExpIncrease = 150;
        static int IncreaseOfExpIncrease = 50;
        static public int GetAccExpForNextLevel(int curLevel)
        {
            if (curLevel <= 0)
                return 0;

            int levelDown = Mathf.Max(0, curLevel - 1);
            int levelDownDown = Mathf.Max(0, curLevel - 2);
            return FirstExpAtLevelOne
            + levelDown * FirstExpIncrease
            + levelDown * levelDownDown / 2 * IncreaseOfExpIncrease;
        }
        static public int GetLevelFromAccExp(float accumulatedExp)
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


        static public int GetAttackableLayerMask(int layer)
        {
            // if (layer == LayerID.Player)
            //     return 1 << LayerID.Enemy;
            // else if (layer == LayerID.Enemy)
            //     return 1 << LayerID.Player;
            // else
                return 0;
        }

        static public int ToSkillSlotIndex(this PlayerUnitInputType type)
        {
            switch (type)
            {
                // case PlayerUnitInputType.SkillSlotA: return 0;
                // case PlayerUnitInputType.SkillSlotB: return 1;
                // case PlayerUnitInputType.SkillSlotC: return 2;
                // case PlayerUnitInputType.SkillSlotD: return 3;
                default: return -1;
            }
        }
        static public PlayerUnitInputType ToSkillSlotEnum(this int slotIndex)
        {
            switch (slotIndex)
            {
                case 0: return PlayerUnitInputType.SkillSlotA;
                case 1: return PlayerUnitInputType.SkillSlotB;
                case 2: return PlayerUnitInputType.SkillSlotC;
                case 3: return PlayerUnitInputType.SkillSlotD;
                default: return PlayerUnitInputType.None;
            }
        }
    }
}