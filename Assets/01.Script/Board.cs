using System;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private Vector2 startPos = new Vector2(-2,2);
    [SerializeField] private Vector2Int boardSize;
    
    [Header("Prefab info")]
    [SerializeField] private GameObject[] tilePrefabs;

    private GameObject[,] tiles;
    
    private int GetBoardWidthSize => boardSize.x;
    private int GetBoardHeightSize => boardSize.y;
        
    private const int TILE_SIZE_WIDTH = 1;
    private const int TILE_SIZE_HEIGHT = 1;
    
    private void Awake()
    {
        CreateTiles();
    }
    
    private void CreateTiles()
    {
        tiles = new GameObject[boardSize.x, boardSize.y];
        
        for (int x = 0; x < GetBoardWidthSize; x++)
        {
            for (int y = 0; y < GetBoardHeightSize; y++)
            {
                Vector2 pos = startPos + new Vector2(x * TILE_SIZE_WIDTH, -y * TILE_SIZE_HEIGHT);
                GameObject tile = Instantiate(tilePrefabs[(y + x) % 2], pos, Quaternion.identity);
                tile.transform.SetParent(transform);
                
                tiles[x, y] = tile;
            }
        }
        
    }

    
    
}
