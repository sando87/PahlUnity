using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine.Events;
using System.Collections.Generic;

namespace PahlBit
{
    public class TriggerComponent : MonoBehaviour
    {
        [SerializeField] InteractableCollider _DetectArea = null;
        [SerializeField] InteractableCollider _TriggerArea = null;

        private BaseObject mBaseObj = null;
        private List<Collider2D> mCollidersInDetectArea = new List<Collider2D>();
        private List<Collider2D> mCollidersInTriggerArea = new List<Collider2D>();
        private bool mIsTriggered = false;

        void Start()
        {
            mBaseObj = this.ExGetBase();
            _DetectArea.OnInteractEnter.AddListener((col) => mCollidersInDetectArea.Add(col));
            _DetectArea.OnInteractLeave.AddListener((col) => mCollidersInDetectArea.Remove(col));
            _TriggerArea.OnInteractEnter.AddListener((col) => mCollidersInTriggerArea.Add(col));
            _TriggerArea.OnInteractLeave.AddListener((col) => mCollidersInTriggerArea.Remove(col));
        }

        void Update()
        {
            if (mIsTriggered)
                return;

            if (InCondition())
            {
                mIsTriggered = true;
                InvokeTrigger();
                enabled = false;
            }
        }

        bool InCondition()
        {
            if (mCollidersInDetectArea.Count > 0)
            {
                BaseObject obj = mCollidersInDetectArea[0].ExGetBase();
                if (obj != null && obj == InGameEngine.Inst.Player)
                {
                    return true;
                }
            }
            return false;
        }

        void InvokeTrigger()
        {
            foreach (Collider2D col in mCollidersInTriggerArea)
            {
                InteractableCollider ic = col.GetComponent<InteractableCollider>();
                if (ic == null)
                    continue;

                ic.InvokeInteractSignal(mBaseObj, InteractMask.TriggerSignal);
            }
        }
    }
}