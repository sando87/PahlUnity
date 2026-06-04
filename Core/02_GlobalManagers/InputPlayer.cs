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
        [SerializeField] string _InputActionMove = "Move";

        private InputManager mInputManager = null;
        private int mInputActionMove = 0;

        public bool IsLoseControl => mInputManager == null;

        void Awake()
        {
            mInputActionMove = InputManager.GetInputActionNameHash(_InputActionMove);
        }

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

        public bool JustPressed(int inputActionNameHash) => mInputManager != null ? mInputManager.JustPressed(inputActionNameHash) : false;
        public bool IsPressing(int inputActionNameHash) => mInputManager != null ? mInputManager.IsPressing(inputActionNameHash) : false;
        public bool JustReleased(int inputActionNameHash) => mInputManager != null ? mInputManager.JustReleased(inputActionNameHash) : false;

        public float MoveX { get => mInputManager != null ? mInputManager.GetInputValue<Vector2>(mInputActionMove).x : 0f; }
        public float MoveY { get => mInputManager != null ? mInputManager.GetInputValue<Vector2>(mInputActionMove).y : 0f; }
        public Vector2 MoveXY { get => mInputManager != null ? mInputManager.GetInputValue<Vector2>(mInputActionMove) : Vector2.zero; }

        public TValue GetInputValue<TValue>(int inputActionNameHash) where TValue : struct
        {
            if (mInputManager != null)
            {
                return mInputManager.GetInputValue<TValue>(inputActionNameHash);
            }
            else
            {
                return default;
            }
        }
    }
}