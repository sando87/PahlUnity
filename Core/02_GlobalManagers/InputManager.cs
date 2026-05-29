using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.LowLevel;

/// <summary>
/// 게임 전반의 입력을 관리
/// </summary>

namespace PahlUnity
{
    public enum InputDeviceType
    {
        None,
        KeyboardMouse,
        Gamepad,
        Touch,
    }
    public interface IInputHandler
    {
        void OnInputEnter(InputManager inputManager) { }
        void OnInputExit(InputManager inputManager) { }
        void OnInputUpdate(InputManager inputManager);
    }

    public class InputManager : SingletonMono<InputManager>
    {
        // =========================
        // Events
        // =========================

        /// <summary>
        /// 어떤 입력이든 발생했을 때 호출
        /// </summary>
        public event Action<InputDevice> OnAnyInputDetected;

        /// <summary>
        /// 입력 장치 연결
        /// </summary>
        public event Action<InputDevice> OnDeviceConnected;

        /// <summary>
        /// 입력 장치 해제
        /// </summary>
        public event Action<InputDevice> OnDeviceDisconnected;

        /// <summary>
        /// 마지막 입력 장치 변경
        /// </summary>
        public event Action<InputDeviceType> OnDeviceTypeChanged;

        // =========================
        // Properties
        // =========================

        public IReadOnlyList<InputDevice> ConnectedDevices => InputSystem.devices;

        public bool IsAnyInputDetectedThisFrame { get; private set; }

        public InputDevice LastUsedDevice { get; private set; }

        public InputDeviceType CurrentDeviceType { get; private set; }

        // =========================
        // Lock States
        // =========================

        public bool IsPlayerInputLocked => _playerInputLockCount > 0;
        public bool IsUIInputLocked => _uiInputLockCount > 0;
        public bool IsAllInputLocked => _allInputLockCount > 0;

        private int _playerInputLockCount;
        private int _uiInputLockCount;
        private int _allInputLockCount;

        private IDisposable _anyButtonPressDisposable;

        // =========================
        // Input Actions
        // =========================

        [Header("Input Actions")]
        [SerializeField]
        private InputActionAsset inputActions;

        [SerializeField]
        private string playerActionMapName = "Player";

        [SerializeField]
        private string uiActionMapName = "UI";

        private InputActionMap _playerMap;
        private InputActionMap _uiMap;
        private Dictionary<InputActionName, InputAction> mActionMap = new();

        private IInputHandler mHandlerInput = null;

        // =========================
        // Unity
        // =========================

        protected override void Awake()
        {
            base.Awake();

            Initialize();
        }

        private void Update()
        {
            IsAnyInputDetectedThisFrame = false;

            if (mHandlerInput != null)
            {
                mHandlerInput.OnInputUpdate(this);
            }
        }

        private void OnDestroy()
        {
            // _anyButtonPressDisposable?.Dispose();
            // _anyButtonPressDisposable = null;

            InputSystem.onDeviceChange -= OnDeviceChange;
            InputSystem.onEvent -= OnDetectInput;
        }

        // =========================
        // Initialize
        // =========================

        private void Initialize()
        {
            _playerMap = inputActions.FindActionMap(playerActionMapName, true);
            _uiMap = inputActions.FindActionMap(uiActionMapName, true);

            InitActionMap();

            InputSystem.onDeviceChange += OnDeviceChange;

            // 전체 입력 감지 방식 1
            //_anyButtonPressDisposable = InputSystem.onAnyButtonPress.Call(OnAnyButtonPress);

            // 전체 입력 감지 방식 2
            // _playerMap.actionTriggered += (context) => OnAnyButtonPress(context.control);

            // 전체 입력 감지 방식 3
            InputSystem.onEvent += OnDetectInput;

            // 전체 입력 감지 방식 4
            // 유니티에서 제공하는 PlayerInput 사용(멀티 플레이 대응 가능)
            // PlayerInput.onActionTriggered += (context) => OnAnyButtonPress(context.control);

            EnableAllMaps();
        }
        public void InitActionMap()
        {
            foreach (InputActionMap map in inputActions.actionMaps)
            {
                foreach (InputAction action in map.actions)
                {
                    InputActionName inputType = new(action.name);
                    mActionMap[inputType] = action;
                }
            }
        }

        // =========================
        // Input Detection
        // =========================

        private void OnAnyButtonPress(InputControl control)
        {
            if (control == null)
                return;

            var device = control.device;
            DetectInputDevice(device);
        }

        private void DetectInputDevice(InputDevice device)
        {
            LastUsedDevice = device;

            var newType = ConvertDeviceType(device);

            if (CurrentDeviceType != newType)
            {
                CurrentDeviceType = newType;
                OnDeviceTypeChanged?.Invoke(CurrentDeviceType);
            }

            IsAnyInputDetectedThisFrame = true;

            OnAnyInputDetected?.Invoke(device);
        }

        private void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            switch (change)
            {
                case InputDeviceChange.Added:
                    OnDeviceConnected?.Invoke(device);
                    break;

                case InputDeviceChange.Removed:
                case InputDeviceChange.Disconnected:
                    OnDeviceDisconnected?.Invoke(device);
                    break;
            }
        }

        private void OnDetectInput(InputEventPtr eventPtr, InputDevice device)
        {
            if (!eventPtr.IsA<StateEvent>() &&
                !eventPtr.IsA<DeltaStateEvent>())
                return;

            DetectInputDevice(device);
        }

        // =========================
        // Device Utilities
        // =========================

        private InputDeviceType ConvertDeviceType(InputDevice device)
        {
            if (device is Gamepad)
                return InputDeviceType.Gamepad;

            if (device is Keyboard || device is Mouse)
                return InputDeviceType.KeyboardMouse;

            if (device is Touchscreen)
                return InputDeviceType.Touch;

            return InputDeviceType.None;
        }

        public bool IsCurrentDeviceGamepad()
        {
            return CurrentDeviceType == InputDeviceType.Gamepad;
        }

        public bool IsCurrentDeviceKeyboardMouse()
        {
            return CurrentDeviceType == InputDeviceType.KeyboardMouse;
        }

        // =========================
        // Lock Functions
        // =========================

        public void LockPlayerInput()
        {
            _playerInputLockCount++;
            RefreshInputState();
        }

        public void UnlockPlayerInput()
        {
            _playerInputLockCount = Mathf.Max(0, _playerInputLockCount - 1);
            RefreshInputState();
        }

        public IDisposable LockPlayerInputScope()
        {
            LockPlayerInput();

            return new ScopeAction(() =>
            {
                UnlockPlayerInput();
            });
        }

        public void LockUIInput()
        {
            _uiInputLockCount++;
            RefreshInputState();
        }

        public void UnlockUIInput()
        {
            _uiInputLockCount = Mathf.Max(0, _uiInputLockCount - 1);
            RefreshInputState();
        }

        public IDisposable LockUIInputScope()
        {
            LockUIInput();

            return new ScopeAction(() =>
            {
                UnlockUIInput();
            });
        }

        public void LockAllInput()
        {
            _allInputLockCount++;
            RefreshInputState();
        }

        public void UnlockAllInput()
        {
            _allInputLockCount = Mathf.Max(0, _allInputLockCount - 1);
            RefreshInputState();
        }

        public IDisposable LockAllInputScope()
        {
            LockAllInput();

            return new ScopeAction(() =>
            {
                UnlockAllInput();
            });
        }

        // =========================
        // Apply Lock State
        // =========================

        private void RefreshInputState()
        {
            if (IsAllInputLocked)
            {
                DisableAllMaps();
                return;
            }

            if (IsPlayerInputLocked)
                _playerMap.Disable();
            else
                _playerMap.Enable();

            if (IsUIInputLocked)
                _uiMap.Disable();
            else
                _uiMap.Enable();
        }

        private void EnableAllMaps()
        {
            _playerMap?.Enable();
            _uiMap?.Enable();
        }

        private void DisableAllMaps()
        {
            _playerMap?.Disable();
            _uiMap?.Disable();
        }

        // =========================
        // Utility
        // =========================

        public bool HasGamepad()
        {
            return Gamepad.current != null;
        }

        public bool HasKeyboard()
        {
            return Keyboard.current != null;
        }

        public bool HasMouse()
        {
            return Mouse.current != null;
        }

        public bool HasTouch()
        {
            return Touchscreen.current != null;
        }

        public bool JustPressed(InputActionName inputType) => GetInputAction(inputType).triggered;
        public bool IsPressing(InputActionName inputType) => GetInputAction(inputType).IsPressed();
        public bool JustReleased(InputActionName inputType) => GetInputAction(inputType).WasReleasedThisFrame();

        public TValue GetInputValue<TValue>(InputActionName inputType) where TValue : struct
        {
            InputAction action = GetInputAction(inputType);
            return action.ReadValue<TValue>();
        }

        private InputAction GetInputAction(InputActionName inputType)
        {
            LOG.errorif(!mActionMap.ContainsKey(inputType));
            return mActionMap[inputType];
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
    }

    public readonly partial struct InputActionName
    {
        public readonly int mVal;
        public InputActionName(string value) => mVal = StableHash.ToInt32(value);
        public InputActionName(int value) => mVal = value;

        public static implicit operator int(InputActionName info) => info.mVal;
        public static implicit operator InputActionName(int val) => new InputActionName(val);
        public static implicit operator InputActionName(string val) => new InputActionName(val);

        public override bool Equals(object obj) => obj is InputActionName other && mVal == other.mVal;
        public override int GetHashCode() => mVal;

        public static bool operator ==(InputActionName a, InputActionName b) => a.mVal == b.mVal;
        public static bool operator !=(InputActionName a, InputActionName b) => a.mVal != b.mVal;

        public static readonly InputActionName UIMove = new("UIMove");
        public static readonly InputActionName UIBack = new("UIBack");
        public static readonly InputActionName Move = new("Move");
        public static readonly InputActionName Jump = new("Jump");
        public static readonly InputActionName Dash = new("Dash");

        // Example
        // public static readonly InputActionName Jump = new("Jump");
        // public static readonly InputActionName Dash = new("Dash");
        // public static readonly InputActionName SkillA = new("SkillA");
        // public static readonly InputActionName SkillB = new("SkillB");
    }
}