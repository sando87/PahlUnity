using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace PahlBit
{
    public class AnimatorHelper : MonoBehaviour
    {
        [SerializeField] AnimatorStateEventSet[] _stateEvents = null;


        private Dictionary<AnimStateNameHash, AnimatorStateEventSet> mAnimatorEvents = new Dictionary<AnimStateNameHash, AnimatorStateEventSet>();

        Animator mAnimator = null;

        void Awake()
        {
            mAnimator = GetComponent<Animator>();

            InitEvents();
        }

        void InitEvents()
        {
            foreach (var eventSet in _stateEvents)
            {
                mAnimatorEvents[eventSet.StateNameHash] = eventSet;
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

        public void CrossFadeToState(string stateName, int layer = 0)
        {
            mAnimator.CrossFade(stateName, 0, layer, 0);
        }
        public void CrossFadeToState(int stateHashName, int layer = 0)
        {
            mAnimator.CrossFade(stateHashName, 0, layer, 0);
        }

        public int GetCurrentStateNameHash(int layer)
        {
            return mAnimator.GetCurrentAnimatorStateInfo(layer).shortNameHash;
        }


        AnimEventState _animEventState = new AnimEventState();
        public AnimEventState PlayAnim(AnimStateNameHash stateNameHash)
        {
            _animEventState.ResetEventState(stateNameHash);
            mAnimator.CrossFade(stateNameHash, 0, 0, 0);
            return _animEventState;
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

        public void FootR()
        {
        }

        public void FootL()
        {
        }

        public void Land()
        {
        }

        public void Shoot()
        {
            int curStateHashName = mAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash;
            InvokeEventMiddle(curStateHashName, 0);
        }


        public void InvokeEventEnter(AnimStateNameHash stateNameHash)
        {
            if (mAnimatorEvents.ContainsKey(stateNameHash))
            {
                mAnimatorEvents[stateNameHash].EventEnter.Invoke();
            }
        }
        public void InvokeEventMiddle(AnimStateNameHash stateNameHash, int index)
        {
            if (mAnimatorEvents.ContainsKey(stateNameHash))
            {
                mAnimatorEvents[stateNameHash].EventMiddle.Invoke(index);
            }

            if (_animEventState.CurrentAnim == stateNameHash)
            {
                _animEventState.IsFired = true;
                _animEventState.FireIndex = index;
            }
        }
        public void InvokeEventLeave(AnimStateNameHash stateNameHash)
        {
            if (mAnimatorEvents.ContainsKey(stateNameHash))
            {
                mAnimatorEvents[stateNameHash].EventLeave.Invoke();
            }

            if (_animEventState.CurrentAnim == stateNameHash)
            {
                _animEventState.IsEnd = true;
            }
        }



        public void AddEventEnter(AnimStateNameHash stateNameHash, UnityAction action)
        {
            if (!mAnimatorEvents.ContainsKey(stateNameHash))
            {
                mAnimatorEvents[stateNameHash] = new AnimatorStateEventSet(stateNameHash);
            }
            mAnimatorEvents[stateNameHash].EventEnter.AddListener(action);
        }
        public void AddEventMiddle(AnimStateNameHash stateNameHash, UnityAction<int> action)
        {
            if (!mAnimatorEvents.ContainsKey(stateNameHash))
            {
                mAnimatorEvents[stateNameHash] = new AnimatorStateEventSet(stateNameHash);
            }
            mAnimatorEvents[stateNameHash].EventMiddle.AddListener(action);
        }
        public void AddEventLeave(AnimStateNameHash stateNameHash, UnityAction action)
        {
            if (!mAnimatorEvents.ContainsKey(stateNameHash))
            {
                mAnimatorEvents[stateNameHash] = new AnimatorStateEventSet(stateNameHash);
            }
            mAnimatorEvents[stateNameHash].EventLeave.AddListener(action);
        }
        public void RemoveEventEnter(AnimStateNameHash stateNameHash, UnityAction action)
        {
            if (mAnimatorEvents.ContainsKey(stateNameHash))
            {
                mAnimatorEvents[stateNameHash].EventEnter.RemoveListener(action);
            }
        }
        public void RemoveEventMiddle(AnimStateNameHash stateNameHash, UnityAction<int> action)
        {
            if (mAnimatorEvents.ContainsKey(stateNameHash))
            {
                mAnimatorEvents[stateNameHash].EventMiddle.RemoveListener(action);
            }
        }
        public void RemoveEventLeave(AnimStateNameHash stateNameHash, UnityAction action)
        {
            if (mAnimatorEvents.ContainsKey(stateNameHash))
            {
                mAnimatorEvents[stateNameHash].EventLeave.RemoveListener(action);
            }
        }

    }

    [System.Serializable]
    public class AnimatorStateEventSet
    {
        [AnimatorStateHash]
        public int StateNameHash = 0;
        public UnityEvent EventEnter = new UnityEvent();
        public UnityEvent<int> EventMiddle = new UnityEvent<int>();
        public UnityEvent EventLeave = new UnityEvent();

        public AnimatorStateEventSet(int stateNameHash)
        {
            StateNameHash = stateNameHash;
        }
    }

    public class AnimEventState
    {
        public AnimStateNameHash CurrentAnim = 0;
        public bool IsFired = false;
        public bool IsEnd = false;
        public int FireIndex = -1;

        public void ResetEventState(AnimStateNameHash animStateNameHash)
        {
            CurrentAnim = animStateNameHash;
            IsFired = false;
            IsEnd = false;
            FireIndex = -1;
        }
    }
}