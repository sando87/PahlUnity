using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public interface IInputHandler
    {
        void OnInputEnter(InputSystemManager inputManager) {}
        void OnInputExit(InputSystemManager inputManager) {}
        void OnInputUpdate(InputSystemManager inputManager);
    }
}