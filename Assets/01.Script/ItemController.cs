using UnityEngine.AddressableAssets;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] private AssetReference levelDataContainerRef;
    
    [Space]
    [SerializeField] private AssetReferenceColorDataContainer colorDataContainer;
    [SerializeField] private AssetReferenceGameObject itemAssetReference;
    
    private Tile[] tiles;
    private ColorDataContainer cachedData;
        
    public async void Init(Tile[] tiles)
    {
        this.tiles = tiles;
        cachedData = await AssetManager.LoadAsync<ColorDataContainer>(colorDataContainer);
    }
      
    public async Task<Tween> CreateItem()
    {
        var levelDataList = await AssetManager.LoadAsync<LevelDataContainer>(levelDataContainerRef);
        var levelDataRef = levelDataList.Get();
        var levelDataAsset = await AssetManager.LoadAsync<LevelData>(levelDataRef);
        
        Tween tween = null;
        int index = 0;
        
        foreach (var currentTile in tiles)
        {
            Vector2 spawnPos = currentTile.transform.position;
            
            var itemRef = await AssetManager.LoadAsync<GameObject>(itemAssetReference); 
            Item currentItem = Instantiate(itemRef, spawnPos, Quaternion.identity).GetComponent<Item>();
            
            tween = currentItem.SetData(levelDataAsset.colorDataList[index++]);
            currentTile.CurrentItem = currentItem;
        }
        
        return tween;
    }
    
    public async Task<Tween> RefillItem()
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
        int randIndex = Random.Range(0, cachedData.itemList.Length);
        return item.SetData(cachedData.itemList[randIndex]);
    }
    
}
