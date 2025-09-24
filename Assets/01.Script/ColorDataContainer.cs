using System;
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
public struct ColorData : IEquatable<ColorData>
{
    public ColorType ColorType;
    public Color Color;


    public bool Equals(ColorData other)
    {
        return ColorType == other.ColorType;
    }
    
    public bool NotEquals(ColorData other)
    {
        return ColorType != other.ColorType;
    }
    
}

[CreateAssetMenu(fileName = "ColorDataContainer", menuName = "SO/ColorDataContainer")]
public class ColorDataContainer : ScriptableObject
{ 
    public ColorData[] itemList;
}
