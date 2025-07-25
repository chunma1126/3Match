using System.Collections;
using UnityEngine;
using TMPro;

public class UIManager : MonoSingleton<UIManager>
{
    [Header("Score info")]
    [SerializeField] private TextMeshProUGUI scoreText;
    private Counter scoreCounter;
    private float score;
    private WaitForSeconds scoreWait;
        
    protected override void Awake()
    {
        base.Awake();
        scoreCounter  = new Counter();
        scoreWait = new WaitForSeconds(0.001f);
        
        scoreCounter.OnChangeValue += ChangeScoreText;
    }
    
    public void AddScore(float amountValue)
    {
        scoreCounter.Add(amountValue);
    }
    
    private void ChangeScoreText(float value)
    {
        StopCoroutine(nameof(ChangeScoreRoutine));
        StartCoroutine(nameof(ChangeScoreRoutine));
    }
    
    private IEnumerator ChangeScoreRoutine()
    {  
        while (score < scoreCounter.Value)
        {
            score += 1f;
            if (score > scoreCounter.Value)
                score = scoreCounter.Value;

            scoreText.text = "Score: " + ((int)score).ToString("D7");
            yield return scoreWait;
        }
    }
        
}
