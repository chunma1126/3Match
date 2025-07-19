using UnityEngine;

public enum FruitType
{
    Apple,
    Banana,
    Blueberry,
    Grape,
    Orange,
    Pear,
    strawberry,
    End
}

[System.Serializable]
public struct FruitData
{
    public FruitType fruitType;
    public Sprite fruitSprite;
}

[CreateAssetMenu(fileName = "FruitDataContainer", menuName = "SO/FruitDataContainer")]
public class FruitDataContainer : ScriptableObject
{
    public FruitData[] fruitData;
}
