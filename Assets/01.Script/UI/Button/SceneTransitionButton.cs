using MaskTransitions;
using UnityEngine;

public class SceneTransitionButton : Button
{
    [SerializeField] private string sceneName;
    protected override void Click()
    {
        TransitionManager.Instance.LoadLevel(sceneName);        
    }
        
}
