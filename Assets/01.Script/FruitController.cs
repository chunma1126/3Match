using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FruitController : MonoBehaviour
{
    [SerializeField] private FruitDataContainer fruitDatas;
    [SerializeField] private Fruit fruit;
    
    public void CreateFruit(Tile[] tiles)
    {
        foreach (var currentTile in tiles)
        {
            Vector2 spawnPos = currentTile.transform.position;
            
            Fruit currentFruit = Instantiate(fruit, spawnPos, Quaternion.identity);
            currentFruit.SetData(fruitDatas.fruitData[Random.Range(0 , fruitDatas.fruitData.Length)]);
            
            currentTile.CurrentFruit = currentFruit;
        }
    }
        
}
