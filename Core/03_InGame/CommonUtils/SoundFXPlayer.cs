using PahlUnity;
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
        AudioManager.Instance.PlaySFXClip(clip);
    }

    public void PlaySFXLoop(AudioClip clip)
    {
        AudioManager.Instance.PlaySFXClip(clip, false, transform);
    }
    public void StopSFXLoop(AudioClip clip)
    {
        AudioManager.Instance.StopSFXClip(clip);
    }
}
