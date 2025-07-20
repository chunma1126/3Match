using UnityEngine;

public class Fruit : MonoBehaviour
{
    public FruitData fruitData;
    private SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public void SetData(FruitData data)
    {
        fruitData = data;

        if (data.fruitType == FruitType.None)
        {
            spriteRenderer.sprite = null;
        }
        else
        {
            spriteRenderer.sprite = fruitData.fruitSprite;
        }
        
        gameObject.name = fruitData.fruitType.ToString();
                
    }
    
}
