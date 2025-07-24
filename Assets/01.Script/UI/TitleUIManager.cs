using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TitleUIManager : MonoSingleton<TitleUIManager>
{   
    [SerializeField] private TextMeshProUGUI energyText;
    private Counter energyCounter;
    private const int MAX_ENERGY_COUNT = 10;
    
    [SerializeField] public AddPopup addPopUp;
    
    protected override void Awake()
    {
        base.Awake();
        energyCounter = new Counter();
        energyCounter.OnChangeValue += ChangeEnergyText;
       
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
            ActiveAddPopUp(true);
            return;
        }
        
        energyCounter.Add(energy);
    }
    
    private void ChangeEnergyText(float value)
    {
        string text = $"{value} / {MAX_ENERGY_COUNT}";
        energyText.SetText(text);
    }

    public void ActiveAddPopUp(bool active)
    {
        if (active)
        {
            addPopUp.PopUp();
        }
        else
        {
            addPopUp.PopDown();
        }
    }
    
}
