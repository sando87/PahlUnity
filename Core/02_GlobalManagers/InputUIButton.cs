using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PahlUnity
{
    public class InputUIButton : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler// , ICancelHandler
    {
        public Action<InputUIButton> EventSelect { get; set; }
        public Action<InputUIButton> EventDeselect { get; set; }
        public Action<InputUIButton> EventSubmit { get; set; }
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