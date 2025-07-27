using System;
using DG.Tweening;
using UnityEngine;

[DontDestroyOnLoad]
public class AudioManager : MonoSingleton<AudioManager>
{
    [SerializeField] private AudioSO titleBGM;
    
    private GameObject bgmEmitter;
    private AudioSource bgmSource;
    
    private void Start()
    {
        bgmEmitter = new GameObject("BGMEmitter");
        bgmSource = bgmEmitter.AddComponent<AudioSource>();
        DontDestroyOnLoad(bgmEmitter);
        
        PlayBGM(titleBGM,3.5f);
    }
    
    public void PlaySound(AudioSO clip)
    {
        GameObject audioEmitter = new GameObject("AudioEmitter");
        AudioSource source = audioEmitter.AddComponent<AudioSource>();

        source.clip = clip.GetAudioClip();
        source.volume = clip.GetVolume();
                
        source.outputAudioMixerGroup = clip.GetAudioMixerGroup();
        
        source.PlayOneShot(clip.GetAudioClip());
        Destroy(audioEmitter, clip.GetAudioClip().length);
        
        /*if (!clip.GetLoop())
        {
            source.PlayOneShot(clip.GetAudioClip());
            Destroy(audioEmitter, clip.GetAudioClip().length);
        }
        else
        {
            source.loop = true;
            source.clip = clip.GetAudioClip();
            source.Play();
        }*/
    }
    
    public void PlayBGM(AudioSO bgm,float duration)
    {
        bgmSource.clip = bgm.GetAudioClip();
        bgmSource.loop = bgm.GetLoop();
        bgmSource.outputAudioMixerGroup = bgm.GetAudioMixerGroup();
        bgmSource.volume = 0;
        
        bgmSource.Play();
        
        DOVirtual.Float(bgmSource.volume ,bgm.GetVolume() ,duration , x =>
        {
            bgmSource.volume = x;
        }).SetLink(bgmEmitter);
    }
    
}
