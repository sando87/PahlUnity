using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public enum PlayerUnitInputType
    {
        None, Jump, Move, Dash,
        PotionA, PotionB,
        SkillSlotA, SkillSlotB, SkillSlotC, SkillSlotD,
        ShowPopupStats, ShowPopupInven, ShowPopupSkill,
        UIMove, UIEnter, UIBack
    }

    public class PlayerUnitInputState
    {
        public bool isPressed = false;
        public bool isPressedPreState = false;
        public float pressedTime = 0f;
        public InputAction inputAction = null;

        public bool justPressed => isPressed && !isPressedPreState;
        public float HeldTime => isPressed ? Time.time - pressedTime : 0f;
    }

    public class InputSystemManager : MonoBehaviour
    {
        [SerializeField] VirtualInput _VirtualInput = null;

        private IInputHandler mHandlerInput = null;
        private PlayerInputActions mInputActions;
        private Dictionary<PlayerUnitInputType, PlayerUnitInputState> mInputStates = new Dictionary<PlayerUnitInputType, PlayerUnitInputState>();

        public bool LockPlayerInput
        {
            get => mInputActions.Player.enabled;
            set
            {
                if (value)
                    mInputActions.Player.Disable();
                else
                    mInputActions.Player.Enable();
            }
        }

        public bool JustPressed(PlayerUnitInputType type)
            => _VirtualInput != null ? _VirtualInput.JustPressed(type) : GetInputAction(type).triggered;
        public bool IsPressing(PlayerUnitInputType type)
            => _VirtualInput != null ? _VirtualInput.IsPressed(type) : GetInputAction(type).IsPressed();
        public bool JustReleased(PlayerUnitInputType type)
            => _VirtualInput != null ? _VirtualInput.JustReleased(type) : GetInputAction(type).WasReleasedThisFrame();

        // public bool JustPressed(PlayerUnitInputType type) { return mInputStates[type].justPressed; }
        // public bool IsPressing(PlayerUnitInputType type) { return mInputStates[type].isPressed; }
        // public float HeldTime(PlayerUnitInputType type) { return mInputStates[type].HeldTime; }

        public float MoveX { get => GetInputValue<Vector2>(PlayerUnitInputType.Move).x; }
        public float MoveY { get => GetInputValue<Vector2>(PlayerUnitInputType.Move).y; }
        public Vector2 MoveXY { get => GetInputValue<Vector2>(PlayerUnitInputType.Move); }

        public TValue GetInputValue<TValue>(PlayerUnitInputType type) where TValue : struct
        {
            // Virtual Input 우선
            if (_VirtualInput != null)
            {
                if (typeof(TValue) == typeof(Vector2))
                {
                    Vector2 v = _VirtualInput.GetVector2(type);
                    if (v != Vector2.zero)
                        return (TValue)(object)v;
                }
                else if (typeof(TValue) == typeof(float))
                {
                    float f = _VirtualInput.GetFloat(type);
                    if (Mathf.Abs(f) > 0.0001f)
                        return (TValue)(object)f;
                }
            }
            else
            {
                // 실제 입력
                InputAction action = mInputStates[type].inputAction;
                return action != null ? action.ReadValue<TValue>() : default;
            }
            return default;
        }

        public UnityEvent<PlayerUnitInputType> EnterInput = new UnityEvent<PlayerUnitInputType>();
        public UnityEvent<PlayerUnitInputType> LeaveInput = new UnityEvent<PlayerUnitInputType>();

        private void Awake()
        {
            mInputActions = new PlayerInputActions();

            InitEnumKeys();
        }

        public IInputHandler GetHandlerInput()
        {
            return mHandlerInput;
        }
        public void SetHandlerInput(IInputHandler handler)
        {
            if (mHandlerInput == null)
            {
                mHandlerInput = handler;
                mHandlerInput.OnInputEnter(this);
            }
            else
            {
                mHandlerInput.OnInputExit(this);
                mHandlerInput = handler;
                mHandlerInput.OnInputEnter(this);
            }
        }
        public void ClearHandlerInput()
        {
            if (mHandlerInput != null)
            {
                mHandlerInput.OnInputExit(this);
                mHandlerInput = null;
            }
        }

        void Update()
        {
            if (mHandlerInput != null)
            {
                mHandlerInput.OnInputUpdate(this);
            }
        }

        void InitEnumKeys()
        {
            foreach (PlayerUnitInputType type in MyUtils.EnumForeach<PlayerUnitInputType>())
            {
                if (type == PlayerUnitInputType.None)
                    continue;

                mInputStates[type] = new PlayerUnitInputState();
                mInputStates[type].inputAction = GetInputAction(type);

                mInputStates[type].inputAction.started += ctx => OnStarted(type);
                mInputStates[type].inputAction.performed += ctx => OnPerformed(type);
                mInputStates[type].inputAction.canceled += ctx => OnCanceled(type);
            }

        }

        private void OnStarted(PlayerUnitInputType type)
        {
            // EnterInput.Invoke(type);

            // mInputStates[type].isPressed = true;
        }
        private void OnPerformed(PlayerUnitInputType type)
        {
            // UpdateInput.Invoke(type);
            mInputStates[type].isPressedPreState = mInputStates[type].isPressed;
            mInputStates[type].isPressed = true;

            if (mInputStates[type].justPressed)
            {
                mInputStates[type].pressedTime = Time.time;
                EnterInput.Invoke(type);
            }
        }
        private void OnCanceled(PlayerUnitInputType type)
        {
            mInputStates[type].isPressedPreState = false;
            mInputStates[type].isPressed = false;
            mInputStates[type].pressedTime = 0;

            LeaveInput.Invoke(type);
        }

        private void OnEnable() => mInputActions.Enable();
        private void OnDisable() => mInputActions.Disable();

        private InputAction GetInputAction(PlayerUnitInputType type)
        {
            switch (type)
            {
                case PlayerUnitInputType.Jump:
                    return mInputActions.Player.Jump;
                case PlayerUnitInputType.Move:
                    return mInputActions.Player.Move;
                case PlayerUnitInputType.Dash:
                    return mInputActions.Player.Dash;
                case PlayerUnitInputType.PotionA:
                    return mInputActions.Player.PotionA;
                case PlayerUnitInputType.PotionB:
                    return mInputActions.Player.PotionB;
                case PlayerUnitInputType.SkillSlotA:
                    return mInputActions.Player.SkillSlotA;
                case PlayerUnitInputType.SkillSlotB:
                    return mInputActions.Player.SkillSlotB;
                case PlayerUnitInputType.SkillSlotC:
                    return mInputActions.Player.SkillSlotC;
                case PlayerUnitInputType.SkillSlotD:
                    return mInputActions.Player.SkillSlotD;
                case PlayerUnitInputType.ShowPopupStats:
                    return mInputActions.UI.PopupStats;
                case PlayerUnitInputType.ShowPopupInven:
                    return mInputActions.UI.PopupInven;
                case PlayerUnitInputType.ShowPopupSkill:
                    return mInputActions.UI.PopupSkill;
                case PlayerUnitInputType.UIMove:
                    return mInputActions.UI.Move;
                case PlayerUnitInputType.UIEnter:
                    return mInputActions.UI.Enter;
                case PlayerUnitInputType.UIBack:
                    return mInputActions.UI.Back;
                default:
                    return null;
            }
        }
    }
}