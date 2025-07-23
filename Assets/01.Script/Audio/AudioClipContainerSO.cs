using UnityEngine;

[CreateAssetMenu(fileName = "AudioSO", menuName = "SO/Audio/Container")]
public class AudioClipContainerSO : ScriptableObject,IAudioClip
{
    public AudioClipSO[] audioClips;
    private AudioClipSO currentClip;
    
    public AudioClip GetAudioClip()
    {
        currentClip = audioClips[Random.Range(0 , audioClips.Length)];
        return currentClip.GetAudioClip();
    }
    
    public float GetVolume()
    {
        return currentClip.GetVolume();
    }

    public bool GetLoop()
    {
        return currentClip.GetLoop();
    }
}
