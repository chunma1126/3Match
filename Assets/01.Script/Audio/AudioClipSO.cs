using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioSO", menuName = "SO/Audio/Clip")]
public class AudioClipSO : ScriptableObject,IAudioClip
{
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private AudioMixer mixer;

    [SerializeField] private bool loop;
    [Range(0,1)] [SerializeField] private float volume;
    
    public AudioClip GetAudioClip()
    {
        return audioClip;
    }

    public float GetVolume()
    {
        return volume;
    }

    public bool GetLoop()
    {
        return loop;
    }
}
