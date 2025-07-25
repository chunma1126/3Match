using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioSO", menuName = "SO/Audio/Clip")]
public class AudioClipSO : AudioSO
{
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private AudioMixerGroup mixer;
    
    [SerializeField] private bool loop;
    [Range(0,1)] [SerializeField] private float volume;
    
    public override AudioClip GetAudioClip()
    {
        return audioClip;
    }

    public override float GetVolume()
    {
        return volume;
    }

    public override bool GetLoop()
    {
        return loop;
    }

    public override AudioMixerGroup GetAudioMixerGroup()
    {
        return mixer;
    }
    
}
