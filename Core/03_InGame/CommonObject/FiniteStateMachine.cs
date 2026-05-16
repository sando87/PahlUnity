using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR;

namespace PahlBit
{
    public class FiniteStateMachine : MonoBehaviour
    {
        public FiniteStateBase CurrentStateForDebug = null;

        private Dictionary<int, StateMachineLayer> mLayers = new Dictionary<int, StateMachineLayer>();

        void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            FiniteStateBase[] states = GetComponentsInChildren<FiniteStateBase>();
            foreach (var state in states)
            {
                state.InitState();

                if (!mLayers.ContainsKey(state.Layer))
                {
                    mLayers[state.Layer] = new StateMachineLayer();
                }

                mLayers[state.Layer].AllStates.Add(state);

                if (state is PlayerStateIdle || state is PlayerStateUpperIdle)
                {
                    mLayers[state.Layer].IdleState = state;
                }
            }
        }

        void Start()
        {
            foreach (var layerSet in mLayers)
            {
                int layer = layerSet.Key;
                mLayers[layer].CurrentState = mLayers[layer].IdleState;
                mLayers[layer].CurrentState.EnterState(null);

                if (layer == 0)
                {
                    CurrentStateForDebug = mLayers[layer].CurrentState;
                }
            }
        }

        void Update()
        {
            UpdateState();
        }
        void FixedUpdate()
        {
            FixedUpdateState();
        }

        public bool TryChangeState(FiniteStateBase newState, object param = null, bool ignorePriority = false)
        {
            // 현재 상태와 바꾸려는 상태가 동일하면 전환시키지 않음
            StateMachineLayer currentLayer = mLayers[newState.Layer];
            if (currentLayer.CurrentState == newState)
                return false;

            // if (!ignorePriority)
            // {
            //     if (newState.Priority < currentLayer.CurrentState.Priority)
            //         return;
            // }

            ChangeState(newState, param);
            return true;
        }

        public void ForceChangeState(FiniteStateBase newState, object param = null)
        {
            // 강제로 상태 전환 로직 구현
            ChangeState(newState, param);
        }


        private void ChangeState(FiniteStateBase newState, object param = null)
        {
            // 상태 전환 로직 구현
            StateMachineLayer currentLayer = mLayers[newState.Layer];
            currentLayer.CurrentState.LeaveState();
            currentLayer.PreviousState = currentLayer.CurrentState;
            currentLayer.CurrentState = newState;
            currentLayer.CurrentState.IsJustEntered = true;
            currentLayer.CurrentState.EnterState(param);

            if (newState.Layer == 0)
            {
                CurrentStateForDebug = newState;
            }
        }

        public bool TryChangeState<T>(object param = null, bool ignorePriority = false) where T : FiniteStateBase
        {
            FiniteStateBase state = FindState<T>();
            if (state != null)
            {
                return TryChangeState(state, param, ignorePriority);
            }
            return false;
        }
        public void ForceChangeState<T>(object param = null) where T : FiniteStateBase
        {
            FiniteStateBase state = FindState<T>();
            if (state != null)
            {
                ForceChangeState(state, param);
            }
        }

        public void TryChangeStateToIdle(int layerIndex)
        {
            StateMachineLayer currentLayer = mLayers[layerIndex];
            TryChangeState(currentLayer.IdleState, null, true);
        }

        public T FindState<T>() where T : FiniteStateBase
        {
            return GetComponentInChildren<T>();
        }

        public FiniteStateBase GetCurrentState(int layerIndex = 0)
        {
            if (mLayers.ContainsKey(layerIndex))
            {
                return mLayers[layerIndex].CurrentState;
            }
            return null;
        }
        public FiniteStateType GetCurrentStateType(int layerIndex = 0)
        {
            if (mLayers.ContainsKey(layerIndex))
            {
                return mLayers[layerIndex].CurrentState.StateType;
            }
            return FiniteStateType.None;
        }

        public void HandleAllStateInput()
        {
            foreach (var layer in mLayers)
            {
                foreach (var state in layer.Value.AllStates)
                {
                    if (layer.Value.CurrentState != state)
                        state.HandleInput();
                }
            }
        }
        public void UpdateState()
        {
            foreach (var layer in mLayers)
            {
                if (layer.Value.CurrentState != null)
                {
                    // 처음 딱 현재 State모션 진입했을때는 UpdateState호출 안해주기 위한 장치
                    if (layer.Value.CurrentState.IsJustEntered)
                    {
                        layer.Value.CurrentState.IsJustEntered = false;
                    }
                    else
                    {
                        layer.Value.CurrentState.UpdateState();
                    }
                }
            }
        }
        public void FixedUpdateState()
        {
            foreach (var layer in mLayers)
            {
                if (layer.Value.CurrentState != null)
                {
                    // 처음 딱 현재 State모션 진입했을때는 UpdateState호출 안해주기 위한 장치
                    if (layer.Value.CurrentState.IsJustEntered)
                    {
                        layer.Value.CurrentState.IsJustEntered = false;
                    }
                    else
                    {
                        layer.Value.CurrentState.FixedUpdateState();
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class StateMachineLayer
    {
        public List<FiniteStateBase> AllStates = new List<FiniteStateBase>();
        public FiniteStateBase PreviousState = null;
        public FiniteStateBase CurrentState = null;
        public FiniteStateBase IdleState = null;
    }
}