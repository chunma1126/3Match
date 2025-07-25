using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitleUIManager : MonoSingleton<TitleUIManager>
{   
    [SerializeField] private TextMeshProUGUI energyText;
 
        
    protected override void Awake()
    {
        base.Awake();
      
        GameManager.Instance.energyCounter.OnChangeValue += ChangeEnergyText;
    }

    private void Start()
    {
        GameManager.Instance.energyCounter.Add(0);
    }
    
    private void OnDestroy()
    {
        GameManager.Instance.energyCounter.OnChangeValue -= ChangeEnergyText;
    }
        
    private void ChangeEnergyText(float value)
    {
        string text = $"{value} / {GameManager.MAX_ENERGY_COUNT}";
        energyText.SetText(text);
    }
    
}
