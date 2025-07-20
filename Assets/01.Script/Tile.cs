using UnityEngine;

public class Tile : MonoBehaviour
{
    private Fruit currentFruit;
    public Fruit CurrentFruit
    {
        get => currentFruit;
        set
        {
            currentFruit = value;
            if (currentFruit != null)
                currentFruit.transform.SetParent(transform);
            
        }
    }
}