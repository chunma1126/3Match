using MaskTransitions;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SceneTransitionButton : Button
{
    [SerializeField] private string sceneName;
    
    [Header("BGM info")]
    [SerializeField] private AssetReference bgm;
    [SerializeField] private float duration;
    
    private bool clicked;
    
    protected override async void Click()
    {
        if(clicked)return;
        
        clicked = true;
        
        var loadAsync = await AssetManager.LoadAsync<AudioSO>(bgm);
        AudioManager.Inst.PlayBGM(loadAsync,duration);
        
        TransitionManager.Instance.LoadLevel(sceneName);
    }
        
}
