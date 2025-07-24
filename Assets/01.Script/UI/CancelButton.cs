using UnityEngine;

public class CancelButton : Button
{
    [SerializeField] private PopupType popupType;
    
    protected override void Click()
    {
        if (TitleUIManager.Instance != null)
        {
            TitleUIManager.Instance.PopDown(popupType);
        }
        else if (UIManager.Instance != null)
        {
            UIManager.Instance.PopDown(popupType);
        }
               
    }
    
}
