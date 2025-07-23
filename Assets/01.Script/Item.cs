using DG.Tweening;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ColorData colorData;

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
    
    public Tween SetData(ColorData data)
    {
        colorData = data;
        
        Tween tween = null;
        
        if (data.colorType == ColorType.None)
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
            spriteRenderer.sprite = colorData.sprite;
            spriteRenderer.color = colorData.Color;
        }
        
        gameObject.name = colorData.colorType.ToString();
        return tween;
        
    }
    
}
