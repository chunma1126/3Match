using UnityEngine;
using UnityEngine.Audio;

public abstract class AudioSO : ScriptableObject
{
    public abstract AudioClip GetAudioClip();
    public abstract float GetVolume();
    public abstract bool GetLoop();
    public abstract AudioMixerGroup GetAudioMixerGroup();
}