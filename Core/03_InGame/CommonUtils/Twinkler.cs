using PahlBit;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Twinkler : MonoBehaviour
{
    [SerializeField] SpriteFlashController _SFCtrl = null;

    public void StartTwinkle()
    {
        _SFCtrl.HitFlash();
    }

}
