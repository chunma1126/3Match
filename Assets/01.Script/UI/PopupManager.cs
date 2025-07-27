using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PopupInfo
{
    public PopupType type;
    public BasePopup popup;
}

public class PopupManager : MonoSingleton<PopupManager>
{
    [SerializeField] private List<PopupInfo> popupList = new List<PopupInfo>();
    private Dictionary<PopupType, BasePopup> popups = new Dictionary<PopupType, BasePopup>();
        
    protected override void Awake()
    {
        base.Awake();
        
        foreach (var item in popupList)
        {
            popups.Add(item.type , item.popup);
        }

        foreach (var item in popups)
        {
            PopDown(item.Key);
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
