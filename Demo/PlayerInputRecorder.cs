using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PahlUnity
{
    public class PlayerInputRecorder : MonoBehaviour
    {
        [SerializeField] InputPlayer _PlayerInput = null;
        [SerializeField] VirtualInput _VirtualInput = null;
        [SerializeField] InputRecordData _RecordData = null;

        public InputPlayer PlayerInput => _PlayerInput;
        public VirtualInput VirtualInput => _VirtualInput;

        public bool IsEditorPlaying() { return Application.isPlaying; }
        public bool IsRecording() { return mIsRecording; }
        public bool IsPlaying() { return mIsPlaying; }

        bool mIsRecording = false;
        bool mIsPlaying = false;
        float mLastInputTime = 0;

        [Button("Start Record")]
        [ShowIf(nameof(IsEditorPlaying))]
        [DisableIf(nameof(IsRecording))]
        void StartRecord()
        {
            mIsRecording = true;
            mLastInputTime = Time.time;
            _RecordData.Clear();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(_RecordData);
#endif
            StartButtonCoroutines();
        }
        [Button("Stop Record")]
        [ShowIf(nameof(IsEditorPlaying))]
        [EnableIf(nameof(IsRecording))]
        void StopRecord()
        {
            mIsRecording = false;
            StopAllCoroutines();

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }
        [Button("Play")]
        [ShowIf(nameof(IsEditorPlaying))]
        [DisableIf(nameof(IsPlaying))]
        void Play()
        {
            mIsPlaying = true;
            StartCoroutine(ReplayCoroutine());
        }
        [Button("Stop")]
        [ShowIf(nameof(IsEditorPlaying))]
        [EnableIf(nameof(IsPlaying))]
        void Stop()
        {
            mIsPlaying = false;
            StopAllCoroutines();
        }

        void StartButtonCoroutines()
        {
            // int enumCount = MyUtils.EnumCount<PlayerUnitInputType>();
            // for (int i = 1; i < enumCount; i++)
            // {
            //     PlayerUnitInputType buttonType = (PlayerUnitInputType)i;
            //     if (buttonType == PlayerUnitInputType.Move)
            //     {
            //         StartCoroutine(ProcessMoveInput());
            //     }
            //     else
            //     {
            //         StartCoroutine(ProcessButtonInput(buttonType));
            //     }
            // }
        }

        IEnumerator ProcessButtonInput(InputActionName buttonType)
        {
            while (true)
            {
                yield return new WaitUntil(() => PlayerInput.IsPressing(buttonType));
                Record(buttonType, true);
                yield return new WaitUntil(() => !PlayerInput.IsPressing(buttonType));
                Record(buttonType, false);
            }
        }
        IEnumerator ProcessMoveInput()
        {
            Vector2 prevMoveXY = Vector2.zero;
            while (true)
            {
                Vector2 curMoveXY = PlayerInput.MoveXY;
                if (!curMoveXY.Equals(prevMoveXY))
                {
                    Record(InputActionName.Move, curMoveXY);
                    prevMoveXY = curMoveXY;
                }
                yield return null;
            }
        }


        void Record(InputActionName buttonType, bool isPressed)
        {
            float currentTime = Time.time;
            float delay = currentTime - mLastInputTime;
            mLastInputTime = currentTime;

            var data = new RecordedInputData
            {
                delayTime = delay,
                buttonType = buttonType,
                isPressed = isPressed
            };

            _RecordData.Add(data);

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(_RecordData);
#endif
        }
        void Record(InputActionName buttonType, Vector2 moveValue)
        {
            float currentTime = Time.time;
            float delay = currentTime - mLastInputTime;
            mLastInputTime = currentTime;

            var data = new RecordedInputData
            {
                delayTime = delay,
                buttonType = buttonType,
                MoveValue = moveValue
            };

            _RecordData.Add(data);

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(_RecordData);
#endif
        }


        IEnumerator ReplayCoroutine()
        {
            foreach (var input in _RecordData.inputs)
            {
                yield return new WaitForSeconds(input.delayTime);

                if (input.buttonType == InputActionName.Move)
                {
                    VirtualInput.SetValue(InputActionName.Move, input.MoveValue);
                }
                else
                {
                    if (input.isPressed)
                        VirtualInput.Press(input.buttonType);
                    else
                        VirtualInput.Release(input.buttonType);
                }
            }

            mIsPlaying = false;
        }
    }

}