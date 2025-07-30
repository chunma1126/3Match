using TMPro;
using UnityEngine;

public class MaxScoreText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI maxScoreText;

    private void Awake()
    {
        int maxScore = PlayerPrefs.GetInt(GameManager.MAX_SCORE_SAVE_KEY);
        maxScoreText.SetText(maxScore.ToString("D7"));
    }
    
}
