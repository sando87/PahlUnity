using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

namespace PahlUnity
{
    public static class GameSystem
    {
        static public void DoSave_UserSaveData()
        {
            SaveManager<SaveDataBase>.Instance.SaveImmediate();
        }

        // 바로 세이브 하지 않고 일정 시간 동안 다시 RequestSave() 호출이 없으면 그때 세이브 하는 방식
        // 골드나 경험치 같이 자주 변경되는 데이터를 위한 세이브 방식
        static public void RequestSave()
        {
            SaveManager<SaveDataBase>.Instance.RequestSave();
        }

        // static public int GetAttackableLayerMask(int layer)
        // {
        //     if (layer == LayerID.Player)
        //         return 1 << LayerID.Enemy;
        //     else if (layer == LayerID.Enemy)
        //         return 1 << LayerID.Player;
        //     else
        //     return 0;
        // }

        // static public int ToSkillSlotIndex(this InputActionName type)
        // {
        //     switch (type)
        //     {
        //         // case PlayerUnitInputType.SkillSlotA: return 0;
        //         // case PlayerUnitInputType.SkillSlotB: return 1;
        //         // case PlayerUnitInputType.SkillSlotC: return 2;
        //         // case PlayerUnitInputType.SkillSlotD: return 3;
        //         default: return -1;
        //     }
        // }
        // static public InputActionName ToSkillSlotEnum(this int slotIndex)
        // {
        //     switch (slotIndex)
        //     {
        //         case 0: return InputActionName.SkillSlotA;
        //         case 1: return InputActionName.SkillSlotB;
        //         case 2: return InputActionName.SkillSlotC;
        //         case 3: return InputActionName.SkillSlotD;
        //         default: return InputActionName.None;
        //     }
        // }
    }
}