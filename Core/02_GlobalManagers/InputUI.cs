using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PahlUnity
{
    public class InputUI : MonoBehaviour, IInputHandler
    {
        [SerializeField] string _InputActionUIMove = "Navigate";
        [SerializeField] string _InputActionUISubmit = "Submit";
        [SerializeField] string _InputActionUIBack = "Cancel";
        [SerializeField] UIPartsHandler[] _UIParts;

        public Action EventCancel { get; set; }
        public Action<UIPartsHandler> EventSubmit { get; set; }

        public UIPartsHandler CurrentSelectedPart { get; private set; }
        public UIPartsHandler[] UIParts { get => _UIParts; }

        private int mInputActionUIMove = 0;
        private int mInputActionUISubmit = 0;
        private int mInputActionUIBack = 0;

        void Awake()
        {
            mInputActionUIMove = InputManager.GetInputActionNameHash(_InputActionUIMove);
            mInputActionUISubmit = InputManager.GetInputActionNameHash(_InputActionUISubmit);
            mInputActionUIBack = InputManager.GetInputActionNameHash(_InputActionUIBack);

            if (_UIParts == null || _UIParts.Length == 0)
                _UIParts = GetComponentsInChildren<UIPartsHandler>();
        }

        public void UpdateUIParts()
        {
            _UIParts = GetComponentsInChildren<UIPartsHandler>();
        }

        public void SelectUIPart(UIPartsHandler part)
        {
            if (IsSelectable(part))
            {
                CurrentSelectedPart = part;
                EventSystem.current.SetSelectedGameObject(part.gameObject);
            }
        }

        public void OnInputUpdate(InputManager inputManager)
        {
            if (inputManager.JustPressed(mInputActionUIMove))
            {
                Vector2 moveDir = inputManager.GetInputValue<Vector2>(mInputActionUIMove);
                if (moveDir.magnitude > 0.1f)
                {
                    Move(moveDir.normalized);
                }
            }
            else if (inputManager.JustPressed(mInputActionUISubmit))
            {
                Submit();
            }
            else if (inputManager.JustPressed(mInputActionUIBack))
            {
                EventCancel?.Invoke();
            }
        }

        void Move(Vector2 dir)
        {
            if (_UIParts == null || _UIParts.Length == 0)
                return;

            GameObject current = EventSystem.current.currentSelectedGameObject;
            if (current == null)
            {
                SelectFirst();
                return;
            }

            RectTransform currentRect = current.GetComponent<RectTransform>();
            Vector3 currentPos = currentRect.position;

            UIPartsHandler best = null;
            float bestScore = float.MaxValue;

            foreach (var btn in _UIParts)
            {
                if (btn.gameObject == current) continue;
                if (!IsSelectable(btn)) continue;

                Vector3 dirToTarget = btn.transform.position - currentPos;

                // 방향 체크 (위쪽인지 등)
                if (Vector2.Dot(dir, dirToTarget.normalized) < 0.5f)
                    continue;

                float distance = dirToTarget.sqrMagnitude;

                if (distance < bestScore)
                {
                    bestScore = distance;
                    best = btn;
                }
            }

            if (best != null)
            {
                CurrentSelectedPart = best;
                EventSystem.current.SetSelectedGameObject(best.gameObject);
            }
        }

        public void SelectFirst()
        {
            if (_UIParts == null || _UIParts.Length == 0)
                return;

            foreach (UIPartsHandler part in _UIParts)
            {
                if (IsSelectable(part))
                {
                    SelectUIPart(part);
                    return;
                }
            }
        }

        private void Submit()
        {
            if (!IsSelectable(CurrentSelectedPart))
            {
                SelectFirst();
            }

            if (!IsSelectable(CurrentSelectedPart))
                return;

            EventSubmit?.Invoke(CurrentSelectedPart);
            ExecuteEvents.Execute(CurrentSelectedPart.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
        }

        private bool IsSelectable(UIPartsHandler part)
        {
            if (part == null || !part.gameObject.activeInHierarchy)
                return false;

            Selectable selectable = part.GetComponent<Selectable>();
            return selectable == null || selectable.IsInteractable();
        }
    }


}