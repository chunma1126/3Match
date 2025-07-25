using System.Collections.Generic;

[System.Serializable]
public struct PopupInfo
{
    public PopupType type;
    public BasePopup popup;
}

public class PopupManager : MonoSingleton<PopupManager>
{
    public List<PopupInfo> popupList = new List<PopupInfo>();
    private Dictionary<PopupType, BasePopup> popups = new Dictionary<PopupType, BasePopup>();
    
    
    protected override void Awake()
    {
        base.Awake();
        
        foreach (var item in popupList)
        {
            popups.Add(item.type , item.popup);
        }    
        
    }

    public void PopUp(PopupType type)
    {
        popups[type].PopUp();
    }
    
    public void PopDown(PopupType type)
    {
        popups[type].PopDown();
    }
    
    
    
}
