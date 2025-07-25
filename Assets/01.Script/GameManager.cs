using System;
using UnityEngine;

[DontDestroyOnLoad]
public class GameManager : MonoSingleton<GameManager>
{
    public Counter energyCounter;
    public const int MAX_ENERGY_COUNT = 10;
    private const string SAVE_KEY = "Energy_Key";
    
    protected override void Awake()
    {
        base.Awake();
                
        energyCounter = new Counter();
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        energyCounter.Add(PlayerPrefs.GetFloat(SAVE_KEY));
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetFloat(SAVE_KEY, energyCounter.Value);
    }
    
    public bool HasEnergy()
    {
        return energyCounter.Value > 0;
    }
    

}