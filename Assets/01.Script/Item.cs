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
                spriteRenderer.color = Color.clear;
            });
            
            tween = sequence;
        }
        else
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(originalScale , scaleDuration).SetLink(gameObject));
            spriteRenderer.sprite = colorData.Sprite;
            
            var color = colorData.Color;
            spriteRenderer.color = new Color(color.r ,color.g ,color.b  ,1);
            
            tween = sequence;
        }
        
        gameObject.name = colorData.colorType.ToString();
        return tween;
    }
       
    public SpriteRenderer GetSpriteRenderer() => spriteRenderer;
    
}
