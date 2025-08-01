using UnityEngine;

[CreateAssetMenu(menuName = "SO/LevelDataContainer")]
public class LevelDataContainer : ScriptableObject
{
    public LevelData[] levelDataList;
    
    public LevelData Get()
    {
        if (levelDataList == null || levelDataList.Length == 0)
        {
            Debug.LogError("ERROR: LevelDataContainer is NULL");
            return null;
        }
        
        return levelDataList[Random.Range(0,levelDataList.Length)];
    }
}
