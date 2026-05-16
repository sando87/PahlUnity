using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PahlUnity
{
    public class InputUI : MonoBehaviour, IInputHandler
    {
        [SerializeField] UIPartsHandler[] _UIParts;

        public Action EventCancel { get; set; }

        public UIPartsHandler CurrentSelectedPart { get; private set; }
        public UIPartsHandler[] UIParts { get => _UIParts; }

        void Awake()
        {
            if (_UIParts == null || _UIParts.Length == 0)
                _UIParts = GetComponentsInChildren<UIPartsHandler>();
        }

        public void UpdateUIParts()
        {
            _UIParts = GetComponentsInChildren<UIPartsHandler>();
        }

        public void SelectUIPart(UIPartsHandler part)
        {
            if (part != null && part.gameObject.activeInHierarchy)
            {
                CurrentSelectedPart = part;
                EventSystem.current.SetSelectedGameObject(part.gameObject);
            }
        }

        public void OnInputUpdate(InputManager inputManager)
        {
            if (inputManager.JustPressed("PlayerUnitInputType.UIMove"))
            {
                Vector2 moveDir = inputManager.GetInputValue<Vector2>("PlayerUnitInputType.UIMove");
                if (moveDir.magnitude > 0.1f)
                {
                    Move(moveDir.normalized);
                }
            }
            else if (inputManager.JustPressed("PlayerUnitInputType.UIBack"))
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
                CurrentSelectedPart = _UIParts[0];
                EventSystem.current.SetSelectedGameObject(_UIParts[0].gameObject);
                return;
            }

            RectTransform currentRect = current.GetComponent<RectTransform>();
            Vector3 currentPos = currentRect.position;

            UIPartsHandler best = null;
            float bestScore = float.MaxValue;

            foreach (var btn in _UIParts)
            {
                if (btn.gameObject == current) continue;
                if (!btn.gameObject.activeInHierarchy) continue;

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
    }


}