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
    public ColorType ColorType;
    public Color Color;
}

[CreateAssetMenu(fileName = "ColorDataContainer", menuName = "SO/ColorDataContainer")]
public class ColorDataContainer : ScriptableObject
{ 
    public ColorData[] itemList;
}
