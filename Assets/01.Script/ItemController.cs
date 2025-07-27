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
    
    public Tween RefillItem()
    {
        Tween tween = null;
        foreach (var currentTile in tiles)
        {
            if (currentTile.CurrentItem.colorData.colorType != ColorType.None)
            {
                continue;
            }
              
            tween = SetRandomItem(currentTile.CurrentItem);
        }
        
        return tween;
    }
    
    [ContextMenu("Set Random Item")]
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
        
    private Tween SetRandomItem(Item item)
    {
        int randIndex = Random.Range(0, colorDatas.fruitData.Length);
        return item.SetData(colorDatas.fruitData[randIndex]);
    }
    
}
