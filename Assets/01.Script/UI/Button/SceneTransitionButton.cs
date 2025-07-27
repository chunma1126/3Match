using MaskTransitions;
using UnityEngine;
using UnityEngine.Rendering;

public class SceneTransitionButton : Button
{
    [SerializeField] private string sceneName;
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
