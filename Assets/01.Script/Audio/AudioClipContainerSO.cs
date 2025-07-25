using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioSO", menuName = "SO/Audio/Container")]
public class AudioClipContainerSO : AudioSO
{
    public AudioClipSO[] audioClips;
    private AudioClipSO currentClip;
    
    public override AudioClip GetAudioClip()
    {
        currentClip = audioClips[Random.Range(0 , audioClips.Length)];
        return currentClip.GetAudioClip();
    }
    
    public override float GetVolume()
    {
        return currentClip.GetVolume();
    }

    public override bool GetLoop()
    {
        return currentClip.GetLoop();
    }

    public override AudioMixerGroup GetAudioMixerGroup()
    {
        return currentClip.GetAudioMixerGroup();
    }
}
