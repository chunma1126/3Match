using UnityEngine;
using System.Linq;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class Board : MonoBehaviour
{
    [SerializeField] private Vector2 startPos = new Vector2(-2,2);
    [SerializeField] private Vector2Int boardSize;
    
    [Header("Prefab info")]
    [SerializeField] private Tile[] tilePrefabs;
    
    public int GetBoardWidthSize => boardSize.x;
    public int GetBoardHeightSize => boardSize.y;
    
    private FruitController fruitController;
    private Tile[] tiles;
    private Vector2[] tilePositions;
    
    private const int TILE_SIZE_WIDTH = 1;
    private const int TILE_SIZE_HEIGHT = 1;
    
    private const float SWAP_DURATION = 0.23f;
    
    private bool isMoving = false;
    private Vector2 mousePos;
    
    private int currentFruitIndex = -1;
    private int lastFruitIndex = -1;
    
    private UniqueQueue<int> fruitQueue = new UniqueQueue<int>(10);
    private MatchChecker matchChecker;
    
    private void Awake()
    {
        fruitController = GetComponent<FruitController>();
    }
    
    private void Start()
    {
        CreateTiles();
        
        fruitController.CreateFruit(tiles);
        matchChecker = new MatchChecker(boardSize,tiles);

        MatchAllTiles();
        
        
    }
    
    private void Update()
    {
        SwapProcess();
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
    
    private void SwapProcess()
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
        bool isTileHashFruit = currentFruitIndex != -1 && lastFruitIndex != -1 &&
                               tiles[currentFruitIndex].CurrentFruit.fruitData.fruitType != FruitType.None &&
                               tiles[lastFruitIndex].CurrentFruit.fruitData.fruitType != FruitType.None;
        
        bool canSwap = isTileHashFruit && (isAdjacentHorizontally || isAdjacentVertically);
        if (canSwap)
        {
            TrySwap();
        }
                
    }
        
    private void TrySwap()
    {
        if (currentFruitIndex == -1 || lastFruitIndex == -1)
        {
            ResetFruitIndex();
            return;
        }
        
        FruitSwap(currentFruitIndex, lastFruitIndex).OnComplete(() =>
        {
            if (matchChecker.IsMatch(currentFruitIndex,lastFruitIndex,ref fruitQueue))
            {
                //match
                FruitMatch();
            }
            else
            {
                //swap undo
                FruitSwap(currentFruitIndex, lastFruitIndex).OnComplete(ResetFruitIndex);
            }
        });
                
    }

    private void FruitMatch()
    {
        if(fruitQueue == null || fruitQueue.Count <= 0)return;
        
        // Swap must start from the minimum index
        fruitQueue = new UniqueQueue<int>(fruitQueue.OrderBy(i => i));
        
        int size = fruitQueue.Count - 1;
        for (int i = 0; i <size; i++)
        {
            int fruitIndex = fruitQueue.Dequeue();
            RemoveFruitAt(fruitIndex);
            fruitQueue.Enqueue(fruitIndex); 
        }
        
        int lastIndex = fruitQueue.Dequeue();
        fruitQueue.Enqueue(lastIndex); 
                
        RemoveFruitAt(lastIndex).OnComplete(() =>
        {
            int total = fruitQueue.Count;
            int completed = 0;
            
            foreach (int index in fruitQueue)
            {
                ApplyGravity(index, () =>
                {
                    completed = GravityComplete(completed, total);
                });
            }
            
            fruitQueue.Clear(); 
        });
        
    }

    private int GravityComplete(int completed, int total)
    {
        completed++;
        if (completed >= total)
        {
            completed = 0;
            MatchAllTiles();
            ResetFruitIndex();
        }
        
        return completed;
    }

    private Tween FruitSwap(int currentIndex,int lastIndex)
    {
        isMoving = true;
        
        Tile tileA = tiles[currentIndex];
        Tile tileB = tiles[lastIndex];
        
        Fruit fruitA = tileA.CurrentFruit;
        Fruit fruitB = tileB.CurrentFruit;
        
        tileA.CurrentFruit = fruitB;
        tileB.CurrentFruit = fruitA;
        
        Sequence sequence = DOTween.Sequence();
        sequence.Append(fruitA.transform.DOLocalMove(Vector3.zero, SWAP_DURATION));
        sequence.Join(fruitB.transform.DOLocalMove(Vector3.zero, SWAP_DURATION));
        
        return sequence;
    }
    
    private void ResetFruitIndex()
    {
        isMoving = false;
        currentFruitIndex = -1;
        lastFruitIndex = -1;
    }
        
    private Tween RemoveFruitAt(int index)
    {
        FruitData fruitData = new FruitData();
        fruitData.fruitType = FruitType.None;
        
        return tiles[index].CurrentFruit.SetData(fruitData);
    }
    
    private void ApplyGravity(int index,Action callback = null)
    {
        int aboveIndex = index - GetBoardWidthSize;
        
        if (index < 0 || aboveIndex < 0)
        {
            callback?.Invoke();
            
            return;
        }
        
        isMoving = true;
        
        FruitSwap(index, aboveIndex).OnComplete(() =>
        {
            ApplyGravity(aboveIndex,callback); 
        });
    }
    
    void MatchAllTiles()
    {
        matchChecker.CheckAllTiles(ref fruitQueue);
        FruitMatch();
    }
    
}
