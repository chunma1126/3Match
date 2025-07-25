using UnityEngine;

[DontDestroyOnLoad]
public class AudioManager : MonoSingleton<AudioManager>
{
    public void PlaySound(AudioSO clip)
    {
        GameObject audioEmitter = new GameObject("AudioEmitter");
        AudioSource source = audioEmitter.AddComponent<AudioSource>();

        source.clip = clip.GetAudioClip();
        source.volume = clip.GetVolume();
        source.outputAudioMixerGroup = clip.GetAudioMixerGroup();
        
        if (!clip.GetLoop())
        {
            source.PlayOneShot(clip.GetAudioClip());
            Destroy(audioEmitter, clip.GetAudioClip().length);
        }
        else
        {
            source.loop = true;
            source.clip = clip.GetAudioClip();
            source.Play();
        }
    }
}
