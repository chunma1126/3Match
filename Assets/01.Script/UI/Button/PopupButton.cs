using UnityEngine;

public class PopupButton : Button
{
    [SerializeField] private PopupType popupType;
    
    protected override void Click()
    {
        PopupManager.Instance.PopUp(popupType);
    }
            
}