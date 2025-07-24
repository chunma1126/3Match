
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public enum PopupType
{
    Setting,
    Add,    
    End,
}

public abstract class BasePopup : MonoBehaviour
{
    [FormerlySerializedAs("popUpType")] [SerializeField] private PopupType popupType;
    [SerializeField] private CanvasGroup popupGroup;
    [SerializeField] private CanvasGroup elementsGroup;

    [Range(0, 2)] [SerializeField] private float popUpFadeDuration;    
    [Range(0, 2)] [SerializeField] private float elementsFadeDuration;
    
    private void Awake()
    {
        popupGroup.alpha = 0;
        popupGroup.interactable = false;
        popupGroup.blocksRaycasts = false;
        
        elementsGroup.alpha = 0;
        elementsGroup.interactable = false;
        elementsGroup.blocksRaycasts = false;
    }
    
    public virtual void PopUp()
    {
        popupGroup.interactable = true;
        popupGroup.blocksRaycasts = true;
                
        popupGroup.DOFade(1, 0.5f).OnComplete(() =>
        {
            elementsGroup.interactable = true;
            elementsGroup.blocksRaycasts = true;
            elementsGroup.DOFade(1, elementsFadeDuration);
        });
    }

    public virtual void PopDown()
    {
        elementsGroup.interactable = false;
        elementsGroup.blocksRaycasts = false;
    
        elementsGroup.DOFade(0, elementsFadeDuration).OnComplete(() =>
        {
            popupGroup.interactable = false;
            popupGroup.blocksRaycasts = false;
            popupGroup.DOFade(0, 0.5f);
        });
    }
}