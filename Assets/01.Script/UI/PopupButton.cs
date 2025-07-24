
using UnityEngine;

public class PopupButton : Button
{
    [SerializeField] private PopupType popupType;
    
    protected override void Click()
    {
        if (TitleUIManager.Instance != null)
        {
            TitleUIManager.Instance.PopUp(popupType);
        }
        else if (UIManager.Instance != null)
        {
            UIManager.Instance.PopUp(popupType);
        }
        
    }
}