using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class ItemController : MonoBehaviour
{

    [SerializeField] private LevelData levelData;
    
    [Space]
    [SerializeField] private ColorDataContainer colorDataContainer;
    [SerializeField] private Item item;

    private Tile[] tiles;
        
    public void Init(Tile[] tiles)
    {
        this.tiles = tiles;
    }
    
    public void CreateItem()
    {
        int index = 0;
        foreach (var currentTile in tiles)
        {
            Vector2 spawnPos = currentTile.transform.position;
            
            Item currentItem = Instantiate(item, spawnPos, Quaternion.identity);
            currentItem.SetData(levelData.colorDataList[index++]);
            
            currentTile.CurrentItem = currentItem;
        }
    }
    
    public Tween RefillItem()
    {
        Tween tween = null;
        foreach (var currentTile in tiles)
        {
            if (currentTile.CurrentItem.colorData.ColorType != ColorType.None)
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
            ColorType = ColorType.None
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
        int randIndex = Random.Range(0, colorDataContainer.itemList.Length);
        return item.SetData(colorDataContainer.itemList[randIndex]);
    }
    
}
