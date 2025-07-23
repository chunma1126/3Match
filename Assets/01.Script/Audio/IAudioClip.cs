using UnityEngine;

public interface IAudioClip
{
    public AudioClip GetAudioClip();
    public float GetVolume();
    public bool GetLoop();
}