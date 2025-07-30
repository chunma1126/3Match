using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class UIManager : MonoSingleton<UIManager>
{
    [Header("Score info")]
    [SerializeField] private TextMeshProUGUI scoreText;
    private float score;
    private Coroutine scoreRoutine;
    private WaitForSeconds scoreWait;
        
    [Header("MoveCount info")]
    [SerializeField] private TextMeshProUGUI moveCountText;
        
    protected override void Awake()
    {
        base.Awake();
        
        scoreWait = new WaitForSeconds(0.001f);
    }
    
    private void Start()
    {
        GameManager.Instance.scoreCounter.OnChangeValue += ChangeScoreText;
        GameManager.Instance.moveCounter.OnChangeValue += ChangeMoveCountText;
        
        ChangeMoveCountText(GameManager.Instance.moveCounter.Value);
    }
    
    private void OnDestroy()
    {
        GameManager.Instance.scoreCounter.OnChangeValue -= ChangeScoreText;
        GameManager.Instance.moveCounter.OnChangeValue -= ChangeMoveCountText;
    }
    
    private void ChangeMoveCountText(float value)
    {
        moveCountText.SetText(value.ToString());
    }
        
    private void ChangeScoreText(float value)
    {
        if (scoreRoutine != null)
            StopCoroutine(scoreRoutine);
        
        scoreRoutine = StartCoroutine(ChangeScoreRoutine(value)); 
    }
    
    private IEnumerator ChangeScoreRoutine(float value)
    {
        while (score < value)
        {
            score += 1f;
            if (score > value)
                score = value;

            scoreText.text = ((int)score).ToString("D7");
            yield return scoreWait;
        }
    }
    
        
}
