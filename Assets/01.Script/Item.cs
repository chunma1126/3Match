using System;
using DG.Tweening;
using UnityEngine;


public enum ItemType
{
    Normal = 0,
    Row,
    Column,
    Square,
    Five
}


public class Item : MonoBehaviour
{
    public ItemType itemType = ItemType.Normal;
    
    public Sprite itemSprite;
    public ColorData colorData;
    
    [Header("Match info")] 
    [Range(0,2)][SerializeField] private float scaleSize;
    [Range(0,1)][SerializeField] private float scaleDuration;
    
    private SpriteRenderer spriteRenderer;
    private Vector3 originalScale;

    private Material material;
    
    private const string ColorKey = "_Color";
    private const string ShinyColorKey = "_ShinyColor";
    private const string ActiveKey = "_Active";
    
    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
        
        originalScale = transform.localScale;
    }

    protected virtual void Start()
    {
        spriteRenderer.sprite = itemSprite;
    }
    
    public SpriteRenderer GetSpriteRenderer() => spriteRenderer;
    
    public Tween SetData(ColorData data)
    {
        colorData = data;
        gameObject.name = data.ColorType.ToString();
        
        Sequence sequence = DOTween.Sequence();
        
        if (data.ColorType == ColorType.None)
        {
            itemType = ItemType.Normal;
            
            sequence.Append(transform.DOScale(scaleSize, scaleDuration)).SetLink(gameObject);
            sequence.Append(transform.DOScale(0f, scaleDuration)).SetLink(gameObject);
            sequence.AppendCallback(() =>
            {
                ActivateShinyMaterial(false);
            }).SetLink(gameObject);
        }
        else
        {
            sequence.Append(transform.DOScale(originalScale, scaleDuration).SetLink(gameObject)).SetLink(gameObject);
            sequence.JoinCallback(() =>
            {
                var color = data.Color;
                material.SetColor(ColorKey, color);
            }).SetLink(gameObject);
            
        }
        
        return sequence;
    }
        
    public void SetItemType(ItemType itemType)
    {
        this.itemType = itemType;
        
        ActivateShinyMaterial(true);
    }
    
    private void ActivateShinyMaterial(bool active)
    {
        if (active)
        {
            material.SetColor(ColorKey, colorData.Color);
            material.SetColor(ShinyColorKey , colorData.Color);
            material.SetFloat(ActiveKey, 1);
        }
        else
        {
            material.SetColor(ShinyColorKey , Color.clear);
            material.SetColor(ColorKey, Color.clear);
            material.SetFloat(ActiveKey, 0);
        }
    }
    
}
