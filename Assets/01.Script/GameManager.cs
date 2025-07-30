using UnityEngine;

[DontDestroyOnLoad]
public class GameManager : MonoSingleton<GameManager>
{
    public const string MAX_SCORE_SAVE_KEY = "MaxScore";
    
    [SerializeField] private float defaultMoveCount = 5;
    
    public Counter moveCounter;
    public Counter scoreCounter;

    private float maxScore = 0;
    
    protected override void Awake()
    {
        base.Awake();
        
        moveCounter = new Counter();
        scoreCounter  = new Counter();
        
        maxScore = PlayerPrefs.GetInt(GameManager.MAX_SCORE_SAVE_KEY);
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        moveCounter.Add(defaultMoveCount);
    }
    
    public bool HasMoveCount => moveCounter.Value > 0;
    
    #region Score
    public void InitScore()
    {
        scoreCounter.Add(-scoreCounter.Value);
    }

    public void SaveScore()
    {
        PlayerPrefs.SetInt(MAX_SCORE_SAVE_KEY, (int)maxScore);
    }
    
    public void AddScore(float score)
    {
        scoreCounter.Add(score);
        
        if (scoreCounter.Value > maxScore)
        {
            maxScore = scoreCounter.Value;
        }
    }
    
    #endregion
    
}