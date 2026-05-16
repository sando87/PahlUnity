using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PahlBit
{
    [CreateAssetMenu(fileName = "InputRecordData", menuName = "PahlBit/Input Record Data")]
    public class InputRecordData : ScriptableObject
    {
        public List<RecordedInputData> inputs = new List<RecordedInputData>();

        public void Clear()
        {
            inputs.Clear();
        }

        public void Add(RecordedInputData data)
        {
            inputs.Add(data);
        }
    }


    [System.Serializable]
    public struct RecordedInputData
    {
        public float delayTime;
        public PlayerUnitInputType buttonType;
        public bool isPressed;
        public Vector2 MoveValue;
    }

}