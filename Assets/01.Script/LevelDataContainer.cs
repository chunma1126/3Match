using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "SO/LevelDataContainer")]
public class LevelDataContainer : ScriptableObject
{
    public AssetReference[] levelDataList;
    
    public AssetReference Get()
    {
        if (levelDataList == null || levelDataList.Length == 0)
        {
            Debug.LogError("ERROR: LevelDataContainer is NULL");
            return null;
        }
        
        return levelDataList[Random.Range(0,levelDataList.Length)];
    }
}
