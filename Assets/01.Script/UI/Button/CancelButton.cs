using UnityEngine;

public class CancelButton : Button
{
    [SerializeField] private PopupType popupType;
    
    protected override void Click()
    {
        PopupManager.Instance.PopDown(popupType);
    }
    
}
