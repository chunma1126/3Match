using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "SO/LevelData")]
public class LevelData : ScriptableObject
{
    public Vector2Int boardSize;
    public ColorData[] colorDataList;
}
