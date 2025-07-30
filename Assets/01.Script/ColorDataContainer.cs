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
public struct ColorData
{
    public ColorType colorType;
    public Color Color;

    public Sprite Sprite
    {
        get => sprite;
        set => sprite = value;
    }
    private Sprite sprite;
}

[CreateAssetMenu(fileName = "ColorDataContainer", menuName = "SO/ColorDataContainer")]
public class ColorDataContainer : ScriptableObject
{ 
    public Sprite itemSprite;
    
    [Space]
    public ColorData[] itemList;

    private void OnValidate()
    {
        for (int i = 0; i < itemList.Length; i++)
        {
            var item = itemList[i];
            item.Sprite = itemSprite;
            itemList[i] = item; 
        }
    }
}
