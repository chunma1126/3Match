using System;
using DG.Tweening;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Sprite itemSprite;
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

    private void Start()
    {
        spriteRenderer.sprite = itemSprite;
    }
    
    public Tween SetData(ColorData data)
    {
        colorData = data;
        gameObject.name = data.ColorType.ToString();
    
        Sequence sequence = DOTween.Sequence();
    
        if (data.ColorType == ColorType.None)
        {
            sequence.Append(transform.DOScale(scaleSize, scaleDuration)).SetLink(gameObject);
            sequence.Append(transform.DOScale(0f, scaleDuration)).SetLink(gameObject);
            sequence.AppendCallback(() => spriteRenderer.color = Color.clear).SetLink(gameObject);
        }
        else
        {
            sequence.Append(transform.DOScale(originalScale, scaleDuration).SetLink(gameObject)).SetLink(gameObject);
            sequence.JoinCallback(() =>
            {
                var color = data.Color;
                spriteRenderer.color = new Color(color.r, color.g, color.b, 1);
            }).SetLink(gameObject);
            
        }
        
        return sequence;
    }

    
    public SpriteRenderer GetSpriteRenderer() => spriteRenderer;
    
}
