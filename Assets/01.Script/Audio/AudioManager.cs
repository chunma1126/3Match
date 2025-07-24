using UnityEngine;

[DontDestroyOnLoad]
public class AudioManager : MonoSingleton<AudioManager>
{
    public void PlaySound(IAudioClip clip)
    {
        GameObject audioEmitter = new GameObject("AudioEmitter");
        AudioSource source = audioEmitter.AddComponent<AudioSource>();
        
        source.volume = clip.GetVolume();
        
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
