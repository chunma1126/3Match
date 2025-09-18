using DG.Tweening;
using DG.Tweening.Core.Enums;
using UnityEngine;


public class ItemController : MonoBehaviour
{

    [SerializeField] private LevelDataContainer levelDataContainer;
    
    [Space]
    [SerializeField] private ColorDataContainer colorDataContainer;
    [SerializeField] private Item item;

    private Tile[] tiles;
        
    public void Init(Tile[] tiles)
    {
        this.tiles = tiles;
    }
    
    public Tween CreateItem()
    {
        Tween tween = null;
        int index = 0;
        
        LevelData levelData = levelDataContainer.Get();
        foreach (var currentTile in tiles)
        {
            Vector2 spawnPos = currentTile.transform.position;
            
            Item currentItem = Instantiate(item, spawnPos, Quaternion.identity);
            tween = currentItem.SetData(levelData.colorDataList[index++]);
            
            currentTile.CurrentItem = currentItem;
        }
        
        return tween;
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
    
    
    public Tween ReRollItem()
    {
        for (var index = 0; index < tiles.Length - 1; index++)
        {
            var currentTile = tiles[index];
            currentTile.CurrentItem.SetData(new ColorData()).OnComplete(() =>
            {
                SetRandomItem(currentTile.CurrentItem);
            });
        }
        var seq = DOTween.Sequence();
        seq.Append(tiles[^1].CurrentItem.SetData(new ColorData()));
        seq.Append(SetRandomItem(tiles[^1].CurrentItem));
        
        return seq;
    }
    
    
    private Tween SetRandomItem(Item item)
    {
        int randIndex = Random.Range(0, colorDataContainer.itemList.Length);
        return item.SetData(colorDataContainer.itemList[randIndex]);
    }
        
}
