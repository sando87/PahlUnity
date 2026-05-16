using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PahlBit
{
    public class UIPartsHandler : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler// , ICancelHandler
    {
        public Action<UIPartsHandler> EventSelect { get; set; }
        public Action<UIPartsHandler> EventDeselect { get; set; }
        public Action<UIPartsHandler> EventSubmit { get; set; }
        // public Action<UIPartsHandler> EventCancel { get; set; }

        public void OnSelect(BaseEventData eventData)
        {
            EventSelect?.Invoke(this);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            EventDeselect?.Invoke(this);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            EventSubmit?.Invoke(this);
        }

        // public void OnCancel(BaseEventData eventData)
        // {
        //     EventCancel?.Invoke(this);
        // }
    }
}