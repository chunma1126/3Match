using DG.Tweening;
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
            SetRandomItem(currentItem);
                        
            currentTile.CurrentItem = currentItem;
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

            SetRandomItem(currentTile.CurrentItem);
        }
    }

    public void ReRollItem()
    {
        var data = new ColorData
        {
            colorType = ColorType.None
        };
        
        for (var index = 0; index < tiles.Length - 1; index++)
        {
            var item = tiles[index];
           
            item.CurrentItem.SetData(data);
        }

        tiles[^1].CurrentItem.SetData(data).OnComplete(() =>
        {
            foreach (var item in tiles)
            {
                SetRandomItem(item.CurrentItem);
            }
        });

        
    }

    private void SetRandomItem(Item item)
    {
        item.SetData(colorDatas.fruitData[Random.Range(0 , colorDatas.fruitData.Length)]);
    }
    
}
