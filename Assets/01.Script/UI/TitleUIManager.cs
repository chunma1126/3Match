using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitleUIManager : MonoSingleton<TitleUIManager>
{   
    [SerializeField] private TextMeshProUGUI energyText;
    private Counter energyCounter;
    private const int MAX_ENERGY_COUNT = 10;
    
    [SerializeField] public BasePopup addPopup;
    [SerializeField] public BasePopup settingsPopup;
    
    private Dictionary<PopupType, BasePopup> popups = new Dictionary<PopupType, BasePopup>();
    
    protected override void Awake()
    {
        base.Awake();
        energyCounter = new Counter();
        energyCounter.OnChangeValue += ChangeEnergyText;
        
        popups.Add(PopupType.Add , addPopup);
        popups.Add(PopupType.Setting , settingsPopup);
        
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        energyCounter.Add(MAX_ENERGY_COUNT);
    }

    private void OnDestroy()
    {
        energyCounter.OnChangeValue -= ChangeEnergyText;
    }
    
    public void AddEnergy(float energy)
    {
        if (energyCounter.Value <= 0 && energy <= 0)
        {
            PopUp(PopupType.Add);
            return;
        }
        
        energyCounter.Add(energy);
    }
    
    private void ChangeEnergyText(float value)
    {
        string text = $"{value} / {MAX_ENERGY_COUNT}";
        energyText.SetText(text);
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
