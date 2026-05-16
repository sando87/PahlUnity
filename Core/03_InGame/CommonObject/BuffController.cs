using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlUnity
{
    public class BuffController : MonoBehaviour
    {
        public SpecOption TotalBuffOption { get; private set; } = new SpecOption();

        Dictionary<int, SpecOption> mBuffs = new Dictionary<int, SpecOption>();

        public void SetBuff(int buffID, SpecOption option)
        {
            if (mBuffs.ContainsKey(buffID))
            {
                SpecOption oldOption = mBuffs[buffID];
                TotalBuffOption.Subtract(oldOption);
                TotalBuffOption.Add(option);
                mBuffs[buffID] = option;
            }
            else
            {
                TotalBuffOption.Add(option);
                mBuffs[buffID] = option;
            }
        }
        public void RemoveBuff(int buffID)
        {
            if (mBuffs.ContainsKey(buffID))
            {
                SpecOption option = mBuffs[buffID];
                TotalBuffOption.Subtract(option);
                mBuffs.Remove(buffID);
            }
        }
        public int AddBuff(SpecOption option, float duration)
        {
            int randomBuffID = (int)DateTime.Now.Ticks;
            SetBuff(randomBuffID, option);
            this.ExDelayedCoroutine(duration, () => RemoveBuff(randomBuffID));
            return randomBuffID;
        }

        // 자주 호출되는 함수는 따로 전용 함수를 만들어서 최적화 시킨다.
        // 모션 속도 제어(감속 or 가속)는 프레임 단위로 자주 호출되는 함수라서 따로 분리시킴
        public void SetMoveSpeedBuff(int buffID, PercentUp moveSpeedUp)
        {
            if (mBuffs.ContainsKey(buffID))
            {
                SpecOption option = mBuffs[buffID];
                TotalBuffOption.MoveSpeedUp -= option.MoveSpeedUp;
                option.MoveSpeedUp = moveSpeedUp;
                TotalBuffOption.MoveSpeedUp += option.MoveSpeedUp;
                TotalBuffOption.IsDirty = true;
            }
            else
            {
                SpecOption option = new SpecOption();
                option.MoveSpeedUp = moveSpeedUp;
                TotalBuffOption.MoveSpeedUp += option.MoveSpeedUp;
                TotalBuffOption.IsDirty = true;
                mBuffs[buffID] = option;
            }
        }
    }
}