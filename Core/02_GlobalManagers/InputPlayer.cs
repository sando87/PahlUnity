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

        public bool JustPressed(InputActionName inputType) => mInputManager != null ? mInputManager.JustPressed(inputType) : false;
        public bool IsPressing(InputActionName inputType) => mInputManager != null ? mInputManager.IsPressing(inputType) : false;
        public bool JustReleased(InputActionName inputType) => mInputManager != null ? mInputManager.JustReleased(inputType) : false;

        public float MoveX { get => mInputManager != null ? mInputManager.GetInputValue<Vector2>(InputActionName.Move).x : 0f; }
        public float MoveY { get => mInputManager != null ? mInputManager.GetInputValue<Vector2>(InputActionName.Move).y : 0f; }
        public Vector2 MoveXY { get => mInputManager != null ? mInputManager.GetInputValue<Vector2>(InputActionName.Move) : Vector2.zero; }

        public TValue GetInputValue<TValue>(InputActionName inputType) where TValue : struct
        {
            if (mInputManager != null)
            {
                return mInputManager.GetInputValue<TValue>(inputType);
            }
            else
            {
                return default;
            }
        }
    }
}