using UnityEngine;

[DontDestroyOnLoad]

public class GameManager : MonoSingleton<GameManager>
{
    protected override void Awake()
    {
        base.Awake();
        
        Application.targetFrameRate = 60;
           
    }
    
}