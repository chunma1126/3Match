using MaskTransitions;
using UnityEngine;

public class SceneTransitionButton : Button
{
    [SerializeField] private string sceneName;
    private bool clicked;
    
    protected override void Click()
    {
        if(clicked)return;
        
        clicked = true;
        TransitionManager.Instance.LoadLevel(sceneName);        
    }
    
}
