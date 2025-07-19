using UnityEngine;

public class Fruit : MonoBehaviour
{
    public FruitData fruitData;
    private SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public void Initialize(FruitData data)
    {
        fruitData = data;
        
        gameObject.name = fruitData.fruitType.ToString();
        spriteRenderer.sprite = fruitData.fruitSprite;
    }
    
}
