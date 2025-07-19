using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private Vector2 startPos = new Vector2(-2,2);
    [SerializeField] private Vector2Int boardSize;
    
    [Header("Prefab info")]
    [SerializeField] private Tile[] tilePrefabs;
    
    private FruitController fruitController;
    private Tile[] tiles;
    private Vector2[] tilePositions;
    
    private int GetBoardWidthSize => boardSize.x;
    private int GetBoardHeightSize => boardSize.y;
    
    private const int TILE_SIZE_WIDTH = 1;
    private const int TILE_SIZE_HEIGHT = 1;
    
    private const float SWAP_DURATION = 0.5f;
        
    private bool isMoving = false;
    private Vector2 mousePos;
    
    private int currentFruitIndex = -1;
    private int lastFruitIndex = -1;
    
    private void Awake()
    {
        fruitController = GetComponent<FruitController>();
    }
    
    private void Start()
    {
        CreateTiles();
        fruitController.CreateFruit(tiles);
    }
    
    private void CreateTiles()
    {
        tiles = new Tile[boardSize.x * boardSize.y];
        tilePositions = new Vector2[boardSize.x * boardSize.y];
        
        for (int x = 0; x < GetBoardWidthSize; x++)
        {
            for (int y = 0; y < GetBoardHeightSize; y++)
            {
                Vector2 pos = startPos + new Vector2(x * TILE_SIZE_WIDTH, -y * TILE_SIZE_HEIGHT);
                Tile tile = Instantiate(tilePrefabs[(y + x) % 2], pos, Quaternion.identity);
                tile.transform.SetParent(transform);
                
                tiles[GetIndex(x,y)] = tile;
                tilePositions[GetIndex(x,y)] = tile.transform.position;
            }
        }
        int GetIndex(int x, int y)
        {
            return y * boardSize.x + x;
        }
        
    }
        
    
    
    private Tile FindTile()
    {
        return tiles
            .OrderBy(tile => Vector2.Distance(mousePos, tile.transform.position))
            .FirstOrDefault()
            ?.GetComponentInChildren<Tile>();
    }

    private void Update()
    {
        TrySwap();
                
    }

    private void TrySwap()
    {
        if(isMoving)return;
        
        if (Input.GetMouseButtonDown(0))
        {
            mousePos = Utility.GetMouseWorldPosition();
            Tile currentTile = FindTile();
            
            currentFruitIndex = Array.IndexOf(tilePositions, currentTile.transform.position);
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            mousePos = Utility.GetMouseWorldPosition();
            Tile lastTile = FindTile();
            
            lastFruitIndex = Array.IndexOf(tilePositions, lastTile.transform.position);
        }
        
        bool isAdjacentHorizontally = Mathf.Abs(currentFruitIndex - lastFruitIndex) == 1;
        bool isAdjacentVertically = Mathf.Abs(currentFruitIndex - lastFruitIndex) == GetBoardWidthSize;
        if (isAdjacentHorizontally || isAdjacentVertically)
        {
            FruitSwap();
        }
        
    }
        
    private void FruitSwap()
    {
        if (currentFruitIndex == -1 || lastFruitIndex == -1) return;

        isMoving = true;
        
        Tile tileA = tiles[currentFruitIndex];
        Tile tileB = tiles[lastFruitIndex];

        Fruit fruitA = tileA.CurrentFruit;
        Fruit fruitB = tileB.CurrentFruit;

        tileA.CurrentFruit = fruitB;
        tileB.CurrentFruit = fruitA;
        
        Sequence sequence = DOTween.Sequence();
        sequence.Append(fruitA.transform.DOLocalMove(Vector3.zero, SWAP_DURATION));
        sequence.Join(fruitB.transform.DOLocalMove(Vector3.zero, SWAP_DURATION));
        sequence.AppendCallback(() =>
        {
            isMoving = false;
        });
                
        currentFruitIndex = -1;
        lastFruitIndex = -1;
    }
    
}
