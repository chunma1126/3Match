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
    
    private ItemController itemController;
    private Tile[] tiles;
    private Vector2[] tilePositions;
    
    private const int TILE_SIZE_WIDTH = 1;
    private const int TILE_SIZE_HEIGHT = 1;
    
    private const float SWAP_DURATION = 0.23f;
    
    private bool isMoving = false;
    private int currentFruitIndex = -1;
    private int lastFruitIndex = -1;
        
    private Vector2 mousePos;
    
    private MatchChecker matchChecker;
    private UniqueQueue<int> fruitQueue;
    
    private void Awake()
    {
        itemController = GetComponent<ItemController>();
        
        CreateTiles();
        matchChecker = new MatchChecker(boardSize,tiles);
        fruitQueue = new UniqueQueue<int>(10);
        
        itemController.Init(tiles);
        itemController.CreateItem();
    }
    
    private void Start()
    {
        CheckMatchAllTiles();
    }
    
    private void Update()
    {
        SwapProcess();
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            itemController.RefillItem();
        }
        
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
                               tiles[currentFruitIndex].CurrentItem.colorData.colorType != ColorType.None &&
                               tiles[lastFruitIndex].CurrentItem.colorData.colorType != ColorType.None;
        
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
        if(fruitQueue.Count <= 0)return;
        
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
                ApplyGravity(index, ()=>
                {
                    completed = GravityComplete(completed, total);
                });    
            }
            fruitQueue.Clear(); 
            
        });
        
    }

    private int GravityComplete(int completed, int total)
    {
        ++completed;
        if (completed >= total)
        {
            completed = 0;
            CheckMatchAllTiles();
            ResetFruitIndex();
        }
        
        return completed;
    }

    private Tween FruitSwap(int currentIndex,int lastIndex)
    {
        isMoving = true;
        
        Tile tileA = tiles[currentIndex];
        Tile tileB = tiles[lastIndex];
        
        Item itemA = tileA.CurrentItem;
        Item itemB = tileB.CurrentItem;
        
        tileA.CurrentItem = itemB;
        tileB.CurrentItem = itemA;
        
        Sequence sequence = DOTween.Sequence();
        sequence.Append(itemA.transform.DOLocalMove(Vector3.zero, SWAP_DURATION));
        sequence.Join(itemB.transform.DOLocalMove(Vector3.zero, SWAP_DURATION));
        
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
        ColorData colorData = new ColorData();
        colorData.colorType = ColorType.None;
        
        return tiles[index].CurrentItem.SetData(colorData);
    }
    
    private void ApplyGravity(int index,Action callback = null)
    {
        int aboveIndex = index - GetBoardWidthSize;
        
        if (aboveIndex < 0)
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
    
    private void CheckMatchAllTiles()
    {
        matchChecker.CheckAllTiles(ref fruitQueue);
        FruitMatch();
    }
    
}
