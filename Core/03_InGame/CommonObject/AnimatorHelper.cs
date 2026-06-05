using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace PahlUnity
{
    public class AnimatorHelper : MonoBehaviour
    {
        Animator mAnimator = null;
        AnimEventState mAnimEventState = null;

        void Awake()
        {
            mAnimator = GetComponent<Animator>();
        }


        public void PlayAnim(int stateNameHash)
        {
            if (mAnimEventState != null)
                mAnimEventState.IsEnd = true;

            mAnimator.CrossFade(stateNameHash, 0, 0, 0);
        }
        public void PlayAnim(int stateNameHash, Action<int> onFire)
        {
            if (mAnimEventState != null)
                mAnimEventState.IsEnd = true;

            mAnimEventState = new AnimEventState(stateNameHash);
            mAnimEventState.onFire = onFire;
            mAnimator.CrossFade(stateNameHash, 0, 0, 0);
        }
        public AnimEventState PlayAnimWithEvent(int stateNameHash)
        {
            if (mAnimEventState != null)
                mAnimEventState.IsEnd = true;

            mAnimEventState = new AnimEventState(stateNameHash);
            mAnimator.CrossFade(stateNameHash, 0, 0, 0);
            return mAnimEventState;
        }

        public async UniTask<bool> PlayAnimWaitFire(int stateNameHash)
        {
            if (mAnimEventState != null)
                mAnimEventState.IsEnd = true;

            mAnimEventState = new AnimEventState(stateNameHash);
            AnimEventState animEventState = mAnimEventState;
            mAnimator.CrossFade(stateNameHash, 0, 0, 0);
            await UniTask.WaitUntil(() => animEventState.IsFired || animEventState.IsEnd,
                                        cancellationToken: animEventState.cancelToken.Token,
                                        cancelImmediately: true);
            return animEventState.IsFired;
        }

        public async UniTask PlayAnimWaitEnd(int stateNameHash, Action<int> onFire = null)
        {
            if (mAnimEventState != null)
                mAnimEventState.IsEnd = true;

            mAnimEventState = new AnimEventState(stateNameHash);
            mAnimEventState.onFire = onFire;
            AnimEventState animEventState = mAnimEventState;
            mAnimator.CrossFade(stateNameHash, 0, 0, 0);
            await UniTask.WaitUntil(() => animEventState.IsEnd,
                                        cancellationToken: animEventState.cancelToken.Token,
                                        cancelImmediately: true);
        }

        public void CancelPreviousAnim()
        {
            if (mAnimEventState != null)
            {
                mAnimEventState.IsEnd = true;
                mAnimEventState.cancelToken.Cancel();
                mAnimEventState.cancelToken.Dispose();
                mAnimEventState = null;
            }
        }

        public void SetParamFloat(string paramName, float value)
        {
            mAnimator.SetFloat(paramName, value);
        }
        public void SetParamInt(string paramName, int value)
        {
            mAnimator.SetInteger(paramName, value);
        }
        public void SetParamBool(string paramName, bool value)
        {
            mAnimator.SetBool(paramName, value);
        }
        public void SetParamTrigger(string paramName)
        {
            mAnimator.SetTrigger(paramName);
        }

        public void SetParamFloat(int paramNameHash, float value)
        {
            mAnimator.SetFloat(paramNameHash, value);
        }
        public void SetParamInt(int paramNameHash, int value)
        {
            mAnimator.SetInteger(paramNameHash, value);
        }
        public void SetParamBool(int paramNameHash, bool value)
        {
            mAnimator.SetBool(paramNameHash, value);
        }
        public void SetParamTrigger(int paramNameHash)
        {
            mAnimator.SetTrigger(paramNameHash);
        }

        public float GetParamFloat(int paramNameHash)
        {
            return mAnimator.GetFloat(paramNameHash);
        }
        public int GetParamInt(int paramNameHash)
        {
            return mAnimator.GetInteger(paramNameHash);
        }
        public bool GetParamBool(int paramNameHash)
        {
            return mAnimator.GetBool(paramNameHash);
        }

        public int GetCurrentStateNameHash(int layer)
        {
            return mAnimator.GetCurrentAnimatorStateInfo(layer).shortNameHash;
        }



        public void Fire()
        {
            int curStateHashName = mAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash;
            InvokeEventMiddle(curStateHashName, 0);
        }

        public void Hit()
        {
            int curStateHashName = mAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash;
            InvokeEventMiddle(curStateHashName, 0);
        }

        public void Hit0()
        {
            int curStateHashName = mAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash;
            InvokeEventMiddle(curStateHashName, 0);
        }
        public void Hit1()
        {
            int curStateHashName = mAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash;
            InvokeEventMiddle(curStateHashName, 1);
        }
        public void Hit2()
        {
            int curStateHashName = mAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash;
            InvokeEventMiddle(curStateHashName, 2);
        }

        public void Shoot()
        {
            int curStateHashName = mAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash;
            InvokeEventMiddle(curStateHashName, 0);
        }


        public void InvokeEventEnter(int stateNameHash)
        {
        }
        public void InvokeEventMiddle(int stateNameHash, int index)
        {
            if (mAnimEventState != null && mAnimEventState.CurrentAnim == stateNameHash)
            {
                mAnimEventState.IsFired = true;
                mAnimEventState.FireIndex = index;
                mAnimEventState.onFire?.Invoke(index);
            }
        }
        public void InvokeEventLeave(int stateNameHash)
        {
            if (mAnimEventState != null && mAnimEventState.CurrentAnim == stateNameHash)
            {
                mAnimEventState.IsEnd = true;
            }
        }
    }

    public class AnimEventState
    {
        public int CurrentAnim = 0;
        public bool IsFired = false;
        public bool IsEnd = false;
        public int FireIndex = -1;
        public Action<int> onFire = null;
        public CancellationTokenSource cancelToken = null;

        public AnimEventState(int animStateNameHash)
        {
            CurrentAnim = animStateNameHash;
            IsFired = false;
            IsEnd = false;
            FireIndex = -1;
            cancelToken = new CancellationTokenSource();
        }
    }
}