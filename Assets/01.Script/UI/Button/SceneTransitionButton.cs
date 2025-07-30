using MaskTransitions;
using UnityEngine;

public class SceneTransitionButton : Button
{
    [SerializeField] private string sceneName;
    
    [Header("BGM info")]
    [SerializeField] private AudioSO bgm;
    [SerializeField] private float duration;
    
    private bool clicked;
    
    protected override void Click()
    {
        if(clicked)return;
        
        clicked = true;
        AudioManager.Instance.PlayBGM(bgm,duration);
        TransitionManager.Instance.LoadLevel(sceneName);
    }
        
}
