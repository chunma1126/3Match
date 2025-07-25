using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitleUIManager : MonoSingleton<TitleUIManager>
{   
    [SerializeField] private TextMeshProUGUI energyText;
    private Counter energyCounter;
    private const int MAX_ENERGY_COUNT = 10;
        
    protected override void Awake()
    {
        base.Awake();
        energyCounter = new Counter();
        energyCounter.OnChangeValue += ChangeEnergyText;
                
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
            PopupManager.Instance.PopDown(PopupType.Add);
            return;
        }
                
        energyCounter.Add(energy);
    }
    
    private void ChangeEnergyText(float value)
    {
        string text = $"{value} / {MAX_ENERGY_COUNT}";
        energyText.SetText(text);
    }

    
    
    
}
