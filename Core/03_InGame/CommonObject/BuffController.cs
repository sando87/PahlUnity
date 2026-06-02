using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlUnity
{
    public class BuffInfo
    {
        public IBuff Buff;
        public float EclipseTime;
    }

    public class BuffController : MonoBehaviour
    {
        List<BuffInfo> mBuffs = new();
        public IReadOnlyList<BuffInfo> Buffs => mBuffs;

        public event Action<IBuff> OnBuffAdded;
        public event Action<IBuff> OnBuffRefreshed;
        public event Action<IBuff> OnBuffRemoved;

        public void ApplyBuff(IBuff buff)
        {
            BuffInfo existingBuff = FindBuff(buff);
            if (existingBuff != null)
            {
                existingBuff.EclipseTime = 0;
                OnBuffRefreshed?.Invoke(buff);
            }
            else
            {
                mBuffs.Add(new BuffInfo { Buff = buff, EclipseTime = 0 });
                OnBuffAdded?.Invoke(buff);
            }
        }
        public void RemoveBuff(IBuff buff)
        {
            BuffInfo existingBuff = FindBuff(buff);
            if (existingBuff != null)
            {
                mBuffs.Remove(existingBuff);
                OnBuffRemoved?.Invoke(buff);
            }
        }
        void Update()
        {
            float deltaTime = Time.deltaTime;
            for (int i = mBuffs.Count - 1; i >= 0; i--)
            {
                BuffInfo buffInfo = mBuffs[i];
                buffInfo.EclipseTime += deltaTime;

                if (buffInfo.EclipseTime >= buffInfo.Buff.Duration)
                {
                    mBuffs.RemoveAt(i);
                    OnBuffRemoved?.Invoke(buffInfo.Buff);
                }
            }
        }
        private BuffInfo FindBuff(IBuff buff)
        {
            return mBuffs.Find(b => b.Buff == buff);
        }
    }
}