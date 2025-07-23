using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] private ColorDataContainer colorDatas;
    [SerializeField] private Item item;

    private Tile[] tiles;
        
    public void Init(Tile[] tiles)
    {
        this.tiles = tiles;
    }
    
    public void CreateItem()
    {
        foreach (var currentTile in tiles)
        {
            Vector2 spawnPos = currentTile.transform.position;
            
            Item currentItem = Instantiate(item, spawnPos, Quaternion.identity);
            currentItem.SetData(colorDatas.fruitData[Random.Range(0 , colorDatas.fruitData.Length)]);
        }
    }
    
    public void RefillItem()
    {
        foreach (var currentTile in tiles)
        {
            if (currentTile.CurrentItem.colorData.colorType != ColorType.None)
            {
                continue;   
            }
                    
            currentTile.CurrentItem.SetData(colorDatas.fruitData[Random.Range(0 , colorDatas.fruitData.Length)]);
        }
    } 
            
}
