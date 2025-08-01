using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "SO/LevelData")]
public class LevelData : ScriptableObject
{
    public Vector2Int boardSize;
    public ColorData[] colorDataList;

    public ColorData[] Copy()
    {
        ColorData[] copy = new ColorData[colorDataList.Length];
        
        for (int i = 0; i < colorDataList.Length; i++)
        {
            copy[i] = new ColorData
            {
                ColorType = colorDataList[i].ColorType,
                Color = colorDataList[i].Color
            };
        }
            
        return copy;
    }
    
}
