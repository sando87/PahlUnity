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
    public class GameInputSystem : SingletonMono<GameInputSystem>
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
        }

        private void OnDestroy()
        {
            // _anyButtonPressDisposable?.Dispose();
            // _anyButtonPressDisposable = null;

            InputSystem.onDeviceChange -= OnDeviceChange;
        }

        // =========================
        // Initialize
        // =========================

        private void Initialize()
        {
            _playerMap = inputActions.FindActionMap(playerActionMapName, true);
            _uiMap = inputActions.FindActionMap(uiActionMapName, true);

            InputSystem.onDeviceChange += OnDeviceChange;

            // 전체 입력 감지 방식 1
            //_anyButtonPressDisposable = InputSystem.onAnyButtonPress.Call(OnAnyButtonPress);

            // 전체 입력 감지 방식 2
            // _playerMap.actionTriggered += (context) => OnAnyButtonPress(context.control);

            // 전체 입력 감지 방식 3
            InputSystem.onEvent += (eventPtr, device) =>
            {
                if (!eventPtr.IsA<StateEvent>() &&
                    !eventPtr.IsA<DeltaStateEvent>())
                    return;

                DetectInputDevice(device);
            };

            // 전체 입력 감지 방식 4
            // 유니티에서 제공하는 PlayerInput 사용(멀티 플레이 대응 가능)
            // PlayerInput.onActionTriggered += (context) => OnAnyButtonPress(context.control);

            EnableAllMaps();
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
    }
}