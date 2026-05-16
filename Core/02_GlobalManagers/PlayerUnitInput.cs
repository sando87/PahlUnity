using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class PlayerUnitInput : MonoBehaviour, IInputHandler
    {
        // public UnityEvent<PlayerUnitInputType> EnterInput = new UnityEvent<PlayerUnitInputType>();
        // public UnityEvent<PlayerUnitInputType> LeaveInput = new UnityEvent<PlayerUnitInputType>();

        private InputSystemManager mInputManager = null;

        public bool IsLoseControl => mInputManager == null;

        public void OnInputEnter(InputSystemManager inputManager)
        {
            mInputManager = inputManager;
        }

        public void OnInputExit(InputSystemManager inputManager)
        {
            mInputManager = null;
        }

        public void OnInputUpdate(InputSystemManager inputManager)
        {
        }

        public bool LockPlayerInput
        {
            get => mInputManager != null ? mInputManager.LockPlayerInput : false;
            set
            {
                if (mInputManager != null)
                {
                    mInputManager.LockPlayerInput = value;
                }
            }
        }

        public bool JustPressed(PlayerUnitInputType type) => mInputManager != null ? mInputManager.JustPressed(type) : false;
        public bool IsPressing(PlayerUnitInputType type) => mInputManager != null ? mInputManager.IsPressing(type) : false;
        public bool JustReleased(PlayerUnitInputType type) => mInputManager != null ? mInputManager.JustReleased(type) : false;

        public float MoveX { get => mInputManager != null ? mInputManager.GetInputValue<Vector2>(PlayerUnitInputType.Move).x : 0f; }
        public float MoveY { get => mInputManager != null ? mInputManager.GetInputValue<Vector2>(PlayerUnitInputType.Move).y : 0f; }
        public Vector2 MoveXY { get => mInputManager != null ? mInputManager.GetInputValue<Vector2>(PlayerUnitInputType.Move) : Vector2.zero; }

        public TValue GetInputValue<TValue>(PlayerUnitInputType type) where TValue : struct
        {
            if (mInputManager != null)
            {
                return mInputManager.GetInputValue<TValue>(type);
            }
            else
            {
                return default;
            }
        }
    }
}