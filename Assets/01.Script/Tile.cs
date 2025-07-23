using UnityEngine;

public class Tile : MonoBehaviour
{
    private Item currentItem;
    public Item CurrentItem
    {
        get => currentItem;
        set
        {
            currentItem = value;
            if (currentItem != null)
                currentItem.transform.SetParent(transform);
            
        }
    }
}