using DG.Tweening;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    public FruitData fruitData;

    [Header("Match info")] 
    [Range(0,2)][SerializeField] private float scaleSize;
    [Range(0,1)][SerializeField] private float scaleDuration;
            
    private SpriteRenderer spriteRenderer;
    private Vector3 originalScale;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
    }
    
    public Tween SetData(FruitData data)
    {
        fruitData = data;
        
        Tween tween = null;

        if (data.fruitType == FruitType.None)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(scaleSize, scaleDuration));
            sequence.Append(transform.DOScale(0f, scaleDuration));
            sequence.AppendCallback(() =>
            {
                spriteRenderer.sprite = null;
            });
            
            tween = sequence;
        }
        else
        {
            transform.localScale = originalScale;
            spriteRenderer.sprite = fruitData.fruitSprite;
        }
        
        gameObject.name = fruitData.fruitType.ToString();
        return tween;
        
    }
    
}
