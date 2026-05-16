using System.Collections.Generic;
using UnityEngine;

namespace PahlBit
{
    public class VirtualInput : MonoBehaviour
    {
        class VirtualState
        {
            public bool isPressed;
            public bool isPressedJust;
            public bool isReleasedJust;
            public float pressedTime;
            public Vector2 vector2Value;
            public float floatValue;
            public Coroutine coroutine;
        }

        private Dictionary<PlayerUnitInputType, VirtualState> mStates
            = new Dictionary<PlayerUnitInputType, VirtualState>();

        void Awake()
        {
            foreach (PlayerUnitInputType type in MyUtils.EnumForeach<PlayerUnitInputType>())
            {
                if (type == PlayerUnitInputType.None) continue;
                mStates[type] = new VirtualState();
            }
        }

        // ===== 버튼 입력 =====
        public void Press(PlayerUnitInputType type)
        {
            var state = mStates[type];
            if (!state.isPressed)
            {
                state.isPressed = true;
                state.isPressedJust = true;
                state.pressedTime = Time.time;
                this.ExAfterFrameCoroutine(() => state.isPressedJust = false);
            }
        }

        public void Release(PlayerUnitInputType type)
        {
            var state = mStates[type];
            state.isPressed = false;
            state.isReleasedJust = true;
            state.pressedTime = 0f;
            if (state.coroutine != null)
            {
                StopCoroutine(state.coroutine);
                state.coroutine = null;
            }
            this.ExAfterFrameCoroutine(() => state.isReleasedJust = false);
        }

        public void Tap(PlayerUnitInputType type, float holdTime = 0.2f)
        {
            var state = mStates[type];
            if (state.isPressed) return;

            Press(type);
            state.coroutine = this.ExDelayedCoroutine(holdTime, () => Release(type));
        }

        // ===== 값 입력 =====
        public void SetValue(PlayerUnitInputType type, Vector2 value)
        {
            var state = mStates[type];
            state.vector2Value = value;

            if (value != Vector2.zero)
                Press(type);
            else
                Release(type);
        }

        public void SetValue(PlayerUnitInputType type, float value)
        {
            var state = mStates[type];
            state.floatValue = value;

            if (Mathf.Abs(value) > 0.0001f)
                Press(type);
            else
                Release(type);
        }

        // ===== 조회 =====
        public bool IsPressed(PlayerUnitInputType type)
            => mStates[type].isPressed;

        public bool JustPressed(PlayerUnitInputType type)
        {
            var s = mStates[type];
            return s.isPressedJust;
        }
        public bool JustReleased(PlayerUnitInputType type)
        {
            var s = mStates[type];
            return s.isReleasedJust;
        }

        public float HeldTime(PlayerUnitInputType type)
        {
            var s = mStates[type];
            return s.isPressed ? Time.time - s.pressedTime : 0f;
        }

        public Vector2 GetVector2(PlayerUnitInputType type)
            => mStates[type].vector2Value;

        public float GetFloat(PlayerUnitInputType type)
            => mStates[type].floatValue;
    }
}
