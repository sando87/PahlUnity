using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using Unity.VisualScripting;

namespace PahlUnity
{
    public class AnimatorHelper : MonoBehaviour
    {
        const int MaxLayerCount = 2;

        Animator mAnimator = null;
        AnimStateEvent[] mAnimStateEvents = new AnimStateEvent[MaxLayerCount];
        int mAnimEventStateIDCounter = 0;
        int mCurrentAnimEventStateID = 0;

        public int CurrentStateNameHash => mAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash;
        public int FireIndex { get; private set; } = -1;

        void Awake()
        {
            mAnimator = GetComponent<Animator>();
        }

        public void PlayAnim(int stateNameHash, int layer = 0)
        {
            CancelPreviousAnim(layer);

            mAnimator.CrossFade(stateNameHash, 0, layer, 0);
        }
        public void PlayAnim(int stateNameHash, Action<int> onFire, Action<bool> onEnd, int layer = 0)
        {
            CancelPreviousAnim(layer);

            AnimStateEvent animStateEvent = new(mAnimEventStateIDCounter++, stateNameHash)
            {
                onFire = onFire,
                onEnd = onEnd,
                Layer = layer
            };
            SetAnimStateEvent(animStateEvent);
            mAnimator.CrossFade(stateNameHash, 0, layer, 0);
        }
        public void PlayAnim(int stateNameHash,
                            int fireNameHash,
                            Action<int> onFire,
                            int endNameHash,
                            Action<bool> onEnd,
                            int layer = 0)
        {
            CancelPreviousAnim(layer);

            AnimStateEvent animStateEvent = new(mAnimEventStateIDCounter++, stateNameHash, fireNameHash, endNameHash)
            {
                onFire = onFire,
                onEnd = onEnd,
                Layer = layer
            };
            SetAnimStateEvent(animStateEvent);
            mAnimator.CrossFade(stateNameHash, 0, layer, 0);
        }
        public AnimStateEvent PlayAnimWithEvent(int stateNameHash, int layer = 0)
        {
            CancelPreviousAnim(layer);

            AnimStateEvent animStateEvent = new(mAnimEventStateIDCounter++, stateNameHash)
            {
                Layer = layer
            };
            SetAnimStateEvent(animStateEvent);
            mAnimator.CrossFade(stateNameHash, 0, layer, 0);
            return animStateEvent;
        }

        public async UniTask<AnimStateEvent> PlayAnimWaitFire(int stateNameHash, int layer = 0)
        {
            CancelPreviousAnim(layer);

            AnimStateEvent animStateEvent = new(mAnimEventStateIDCounter++, stateNameHash)
            {
                Layer = layer
            };
            SetAnimStateEvent(animStateEvent);
            mAnimator.CrossFade(stateNameHash, 0, layer, 0);
            await UniTask.WaitUntil(() => animStateEvent.IsFired || animStateEvent.IsEnd,
                                        cancellationToken: animStateEvent.cancelToken.Token,
                                        cancelImmediately: true);
            return animStateEvent;
        }

        public async UniTask<AnimStateEvent> PlayAnimWaitEnd(int stateNameHash, Action<int> onFire = null, int layer = 0)
        {
            CancelPreviousAnim(layer);

            AnimStateEvent animStateEvent = new(mAnimEventStateIDCounter++, stateNameHash)
            {
                onFire = onFire,
                Layer = layer
            };
            SetAnimStateEvent(animStateEvent);
            mAnimator.CrossFade(stateNameHash, 0, layer, 0);
            await UniTask.WaitUntil(() => animStateEvent.IsEnd,
                                        cancellationToken: animStateEvent.cancelToken.Token,
                                        cancelImmediately: true);
            return animStateEvent;
        }

        public void CancelPreviousAnim(int layer)
        {
            if (mAnimStateEvents[layer] != null)
            {
                mAnimStateEvents[layer].Cancel();
                mAnimStateEvents[layer] = null;
            }
        }

        public void CancelAndThrowException(int layer)
        {
            if (mAnimStateEvents[layer] != null)
            {
                mAnimStateEvents[layer].Cancel(true);
                mAnimStateEvents[layer] = null;
            }
        }

        public void SetAnimStateEvent(AnimStateEvent animEventState)
        {
            int layer = animEventState.Layer;
            mAnimStateEvents[layer] = animEventState;
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
        public void SetLayerWeight(int layer, float weight)
        {
            mAnimator.SetLayerWeight(layer, weight);
        }



        public void Fire()
        {
            int curStateHashName = mAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash;
            InvokeEventMiddle(curStateHashName, 0, 0);
        }

        public void HitAtLayer(int layer)
        {
            int curStateHashName = mAnimator.GetCurrentAnimatorStateInfo(layer).shortNameHash;
            InvokeEventMiddle(curStateHashName, 0, layer);
        }

        public void Hit()
        {
            int curStateHashName = mAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash;
            InvokeEventMiddle(curStateHashName, 0, 0);
        }

        public void Hit0()
        {
            int curStateHashName = mAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash;
            InvokeEventMiddle(curStateHashName, 0, 0);
        }
        public void Hit1()
        {
            int curStateHashName = mAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash;
            InvokeEventMiddle(curStateHashName, 1, 0);
        }
        public void Hit2()
        {
            int curStateHashName = mAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash;
            InvokeEventMiddle(curStateHashName, 2, 0);
        }

        public void Shoot()
        {
            int curStateHashName = mAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash;
            InvokeEventMiddle(curStateHashName, 0, 0);
        }


        public void InvokeEventEnter(int stateNameHash, int layer)
        {
            FireIndex = -1;

            if (mAnimStateEvents[layer] != null
            && mAnimStateEvents[layer].StartStateNameHash == stateNameHash
            && mAnimStateEvents[layer].Layer == layer)
            {
                mCurrentAnimEventStateID = mAnimStateEvents[layer].AnimEventID;
            }
        }
        public void InvokeEventMiddle(int stateNameHash, int index, int layer)
        {
            FireIndex = index;

            if (mAnimStateEvents[layer] != null
            && mAnimStateEvents[layer].FireStateNameHash == stateNameHash
            && mAnimStateEvents[layer].AnimEventID == mCurrentAnimEventStateID)
            {
                mAnimStateEvents[layer].IsFired = true;
                mAnimStateEvents[layer].FireIndex = index;
                mAnimStateEvents[layer].onFire?.Invoke(index);
            }
        }
        public void InvokeEventLeave(int stateNameHash, int layer)
        {
            FireIndex = -1;

            if (mAnimStateEvents[layer] != null
            && mAnimStateEvents[layer].EndStateNameHash == stateNameHash
            && mAnimStateEvents[layer].AnimEventID == mCurrentAnimEventStateID
            && mAnimStateEvents[layer].Layer == layer)
            {
                mAnimStateEvents[layer].IsEnd = true;
                mAnimStateEvents[layer].onEnd?.Invoke(false);
                mAnimStateEvents[layer] = null;
            }
        }
    }

    public class AnimStateEvent
    {
        public int AnimEventID = 0;
        public int StartStateNameHash = 0;
        public int FireStateNameHash = 0;
        public int EndStateNameHash = 0;
        public bool IsFired = false;
        public bool IsCanceled = false;
        public bool IsEnd = false;
        public int FireIndex = -1;
        public int Layer = 0;
        public Action<int> onFire = null;
        public Action<bool> onEnd = null;
        public CancellationTokenSource cancelToken = null;

        public AnimStateEvent(int animEventID, int animStateNameHash)
        {
            AnimEventID = animEventID;
            StartStateNameHash = animStateNameHash;
            FireStateNameHash = animStateNameHash;
            EndStateNameHash = animStateNameHash;
            IsFired = false;
            IsCanceled = false;
            IsEnd = false;
            FireIndex = -1;
            Layer = 0;
            cancelToken = new CancellationTokenSource();
        }

        public AnimStateEvent(int animEventID, int startStateNameHash, int fireStateNameHash, int endStateNameHash)
        {
            AnimEventID = animEventID;
            StartStateNameHash = startStateNameHash;
            FireStateNameHash = fireStateNameHash;
            EndStateNameHash = endStateNameHash;
            IsFired = false;
            IsCanceled = false;
            IsEnd = false;
            FireIndex = -1;
            Layer = 0;
            cancelToken = new CancellationTokenSource();
        }

        public void Cancel(bool throwCancelException = false)
        {
            IsCanceled = true;
            IsEnd = true;
            onEnd?.Invoke(IsCanceled);
            if (throwCancelException)
            {
                if (cancelToken != null)
                {
                    cancelToken.Cancel();
                    cancelToken.Dispose();
                    cancelToken = null;
                }
            }
        }
    }
}