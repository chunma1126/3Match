using UnityEngine;
using UnityEngine.Serialization;

public enum ColorType
{
    None,
    Red,
    Orange,
    Yellow,
    Green,
    Blue,
    Navy,
    Purple,
    End
}

[System.Serializable]
public struct ColorData
{
    public ColorType colorType;
    public Color Color;
    [FormerlySerializedAs("fruitSprite")] public Sprite sprite;
}

[CreateAssetMenu(fileName = "ColorDataContainer", menuName = "SO/ColorDataContainer")]
public class ColorDataContainer : ScriptableObject
{
    public ColorData[] fruitData;
}
