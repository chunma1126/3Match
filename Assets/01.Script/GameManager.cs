using UnityEngine;

[DontDestroyOnLoad]
public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private float defaultMoveCount = 5;
    
    public Counter moveCounter;
    public Counter scoreCounter;
        
    protected override void Awake()
    {
        base.Awake();
        
        moveCounter = new Counter();
        scoreCounter  = new Counter();
        
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        moveCounter.Add(defaultMoveCount);
    }
    
    public bool HasMoveCount => moveCounter.Value > 0;

    public void AddScore(float score)
    {
        scoreCounter.Add(score);
    }
    
}