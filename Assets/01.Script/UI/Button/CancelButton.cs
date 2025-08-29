using UnityEngine;

public class CancelButton : Button
{
    [SerializeField] private PopupType popupType;
    
    protected override void Click()
    {
        PopupManager.Inst.PopDown(popupType);
    }
    
}
