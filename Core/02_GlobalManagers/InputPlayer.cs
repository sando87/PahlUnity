using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PahlUnity
{
    public class InputPlayer : MonoBehaviour, IInputHandler
    {
        // public UnityEvent<PlayerUnitInputType> EnterInput = new UnityEvent<PlayerUnitInputType>();
        // public UnityEvent<PlayerUnitInputType> LeaveInput = new UnityEvent<PlayerUnitInputType>();

        private InputManager mInputManager = null;

        public bool IsLoseControl => mInputManager == null;

        public void OnInputEnter(InputManager inputManager)
        {
            mInputManager = inputManager;
        }

        public void OnInputExit(InputManager inputManager)
        {
            mInputManager = null;
        }

        public void OnInputUpdate(InputManager inputManager)
        {
        }

        public bool LockPlayerInput
        {
            get => mInputManager != null ? mInputManager.IsPlayerInputLocked : false;
            set
            {
                if (mInputManager != null)
                {
                    if (value)
                        mInputManager.LockPlayerInput();
                    else
                        mInputManager.UnlockPlayerInput();
                }
            }
        }

        public bool JustPressed(string actionName) => mInputManager != null ? mInputManager.JustPressed(actionName) : false;
        public bool IsPressing(string actionName) => mInputManager != null ? mInputManager.IsPressing(actionName) : false;
        public bool JustReleased(string actionName) => mInputManager != null ? mInputManager.JustReleased(actionName) : false;

        public float MoveX { get => mInputManager != null ? mInputManager.GetInputValue<Vector2>("player/move").x : 0f; }
        public float MoveY { get => mInputManager != null ? mInputManager.GetInputValue<Vector2>("player/move").y : 0f; }
        public Vector2 MoveXY { get => mInputManager != null ? mInputManager.GetInputValue<Vector2>("player/move") : Vector2.zero; }

        public TValue GetInputValue<TValue>(string actionName) where TValue : struct
        {
            if (mInputManager != null)
            {
                return mInputManager.GetInputValue<TValue>(actionName);
            }
            else
            {
                return default;
            }
        }
    }
}