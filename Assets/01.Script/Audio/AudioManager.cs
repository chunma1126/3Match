using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;

[DontDestroyOnLoad]
public class AudioManager : MonoSingleton<AudioManager>
{
    [SerializeField] private AssetReference titleBGM;
    
    private GameObject bgmEmitter;
    private AudioSource bgmSource;
    
    private const string AUDIO_EMITTER_NAME = "AudioEmitter";
    
    private async void Start()
    {
        bgmEmitter = new GameObject("BGMEmitter");
        bgmSource = bgmEmitter.AddComponent<AudioSource>();
        DontDestroyOnLoad(bgmEmitter);
        
        await Task.Yield(); 
        var bgm = await AssetManager.LoadAsync<AudioSO>(titleBGM);
        PlayBGM(bgm, 3.5f);
    }
    
    public void PlaySound(AudioSO clip)
    {
        GameObject audioEmitter = new GameObject(AUDIO_EMITTER_NAME);
        AudioSource source = audioEmitter.AddComponent<AudioSource>();
        
        source.clip = clip.GetAudioClip();
        source.volume = clip.GetVolume();
        
        source.outputAudioMixerGroup = clip.GetAudioMixerGroup();
        
        source.PlayOneShot(clip.GetAudioClip());
        Destroy(audioEmitter, clip.GetAudioClip().length);
        
    }
    
    public void PlayBGM(AudioSO bgm,float duration)
    {
#if UNITY_EDITOR
        if (bgm == null) return;
#endif
        
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
