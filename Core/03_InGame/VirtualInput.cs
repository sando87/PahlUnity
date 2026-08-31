using System.Collections.Generic;
using UnityEngine;

namespace PahlUnity
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

        private Dictionary<int, VirtualState> mStates
            = new Dictionary<int, VirtualState>();

        void Awake()
        {
            // foreach (PlayerUnitInputType type in MyUtils.EnumForeach<PlayerUnitInputType>())
            // {
            //     if (type == PlayerUnitInputType.None) continue;
            //     mStates[type] = new VirtualState();
            // }
        }

        // ===== 버튼 입력 =====
        public void Press(int inputAction)
        {
            var state = mStates[inputAction];
            if (!state.isPressed)
            {
                state.isPressed = true;
                state.isPressedJust = true;
                state.pressedTime = Time.time;
                this.ExAfterFrameCoroutine(() => state.isPressedJust = false);
            }
        }

        public void Release(int inputAction)
        {
            var state = mStates[inputAction];
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

        public void Tap(int inputAction, float holdTime = 0.2f)
        {
            var state = mStates[inputAction];
            if (state.isPressed) return;

            Press(inputAction);
            state.coroutine = this.ExDelayedCoroutine(holdTime, () => Release(inputAction));
        }

        // ===== 값 입력 =====
        public void SetValue(int inputAction, Vector2 value)
        {
            var state = mStates[inputAction];
            state.vector2Value = value;

            if (value != Vector2.zero)
                Press(inputAction);
            else
                Release(inputAction);
        }

        public void SetValue(int inputAction, float value)
        {
            var state = mStates[inputAction];
            state.floatValue = value;

            if (Mathf.Abs(value) > 0.0001f)
                Press(inputAction);
            else
                Release(inputAction);
        }

        // ===== 조회 =====
        public bool IsPressed(int inputAction)
            => mStates[inputAction].isPressed;

        public bool JustPressed(int inputAction)
        {
            var s = mStates[inputAction];
            return s.isPressedJust;
        }
        public bool JustReleased(int inputAction)
        {
            var s = mStates[inputAction];
            return s.isReleasedJust;
        }

        public float HeldTime(int inputAction)
        {
            var s = mStates[inputAction];
            return s.isPressed ? Time.time - s.pressedTime : 0f;
        }

        public Vector2 GetVector2(int inputAction)
            => mStates[inputAction].vector2Value;

        public float GetFloat(int inputAction)
            => mStates[inputAction].floatValue;
    }
}
