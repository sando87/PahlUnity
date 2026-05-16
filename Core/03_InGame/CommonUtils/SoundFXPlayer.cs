using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class SoundFXPlayer : MonoBehaviour
{
    [SerializeField] bool AutoPlayOnEnabled = false;
    [SerializeField] AudioClip _Clip = null;

    void OnEnable()
    {
        if (AutoPlayOnEnabled)
        {
            PlaySFX(_Clip);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        SoundPlayManager.Instance.PlaySFXClip(clip);
    }

    public void PlaySFXLoop(AudioClip clip)
    {
        SoundPlayManager.Instance.PlaySFXClip(clip, false, transform);
    }
    public void StopSFXLoop(AudioClip clip)
    {
        SoundPlayManager.Instance.StopSFXClip(clip);
    }
}
