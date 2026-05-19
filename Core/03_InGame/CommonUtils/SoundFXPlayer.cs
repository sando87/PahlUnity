using PahlUnity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SoundFXPlayer : MonoBehaviour
{
    [SerializeField] bool _AutoPlayOnEnabled = false;
    [SerializeField] AudioClip _Clip = null;
    [SerializeField] AudioClipEx _ClipEx = null;

    void OnEnable()
    {
        if (_AutoPlayOnEnabled)
        {
            if (_ClipEx != null)
                PlaySFX(_ClipEx);
            else if (_Clip != null)
                PlaySFX(_Clip);
        }
    }


    public void PlaySFX(AudioClipEx clipEx)
    {
        AudioManager.Instance.PlayAudioClipData(clipEx);
    }
    public void PlaySFXLoop(AudioClipEx clipEx)
    {
        AudioManager.Instance.PlayAudioClipData(clipEx, transform);
    }
    public void StopSFXLoop(AudioClipEx clipEx)
    {
        AudioManager.Instance.StopAudioClipData(clipEx);
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
