using DG.Tweening;
using UnityEngine;
using Sequence = Unity.VisualScripting.Sequence;

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
        var data = new ColorData { ColorType = ColorType.None };

        var sequence = DOTween.Sequence();
        
        for (int i = 0; i < tiles.Length - 1; i++)
        {
            sequence.Join(tiles[i].CurrentItem.SetData(data));
        }
        
        Tween lastSetDataTween = tiles[^1].CurrentItem.SetData(data);

        
        lastSetDataTween.OnComplete(() =>
        {
            SetRandomItem(tiles[^1].CurrentItem);
            for (int i = 0; i < tiles.Length - 1; i++)
            {
                SetRandomItem(tiles[i].CurrentItem);
            }
        });
        
        sequence.Join(lastSetDataTween);
        
        return sequence;
    }

        
    private Tween SetRandomItem(Item item)
    {
        int randIndex = Random.Range(0, colorDataContainer.itemList.Length);
        return item.SetData(colorDataContainer.itemList[randIndex]);
    }
        
}
