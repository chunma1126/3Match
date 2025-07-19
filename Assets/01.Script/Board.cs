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
    
    private const float SWAP_DURATION = 0.35f;
        
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
    
    private void Update()
    {
        TrySwap();
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
    
    private bool TrySwap()
    {
        if(isMoving)return false;
        
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
        
        return true;
    }
        
    private void FruitSwap()
    {
        if (currentFruitIndex == -1 || lastFruitIndex == -1)
        {
            SwapComplete();
            return;
        }
        
        Swap(() =>
        {
            if (TryMatch() == false)
            {
                Swap(SwapComplete);
            }
            else
            {
                SwapComplete();
            }
        });
    }

    private void Swap(Action callback = null)
    {
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
            callback?.Invoke();
        });
    }
    
    private void SwapComplete()
    {
        Debug.Log("Swap Complete");
        
        isMoving = false;
        currentFruitIndex = -1;
        lastFruitIndex = -1;
    }
    
    private bool TryMatch()
    {
        if (CheckHorizontal(currentFruitIndex)) return true;
        if (CheckHorizontal(lastFruitIndex)) return true;
        
        if (CheckVertical(currentFruitIndex)) return true;
        if (CheckVertical(lastFruitIndex)) return true;
        
        return false;
    }

    private bool CheckHorizontal(int index)
    {
        //left
        {
            int currentX = index;
            int left = currentX - 1;
            int left2 = currentX - 2;
            
            if (IsTripleMatch(currentX , left ,left2))
            {
                return true;
            }
        }
        
        //center
        {
            int currentX = index;
            int left = currentX - 1;
            int right = currentX + 1;
            
            if (IsTripleMatch(currentX , left ,right))
            {
                return true;
            }
        }
        
        //Right
        {
            int currentX = index;
            int right1 = currentX + 1;
            int right2 = currentX + 2;
            
            if (IsTripleMatch(currentX , right1 ,right2))
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckVertical(int index)
    {
        //up
        {
            int currentY = index;
            int up1 = currentY - GetBoardWidthSize;
            int up2 = currentY - GetBoardWidthSize  * 2;
            
            if (IsTripleMatch(currentY , up1 ,up2))
            {
                return true;
            }
        }
        
        //center
        {
            int currentX = index;
            int up = currentX - GetBoardWidthSize;
            int down = currentX + GetBoardWidthSize;
            
            if (IsTripleMatch(currentX , up ,down))
            {
                return true;
            }
        }
        
        //Right
        {
            int currentX = index;
            int down = currentX + GetBoardWidthSize;
            int down2 = currentX + GetBoardWidthSize * 2;
            
            if (IsTripleMatch(currentX , down ,down2))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsTripleMatch(int a, int b, int c)
    {
        int maxWidth = tiles.Length;
        if (a < 0 || b < 0 || c < 0 || a >= maxWidth || b >= maxWidth || c >= maxWidth)
            return false;
        
        var typeA = tiles[a].CurrentFruit.fruitData.fruitType;
        var typeB = tiles[b].CurrentFruit.fruitData.fruitType;
        var typeC = tiles[c].CurrentFruit.fruitData.fruitType;
        
        bool isMatch = typeA == typeB && typeC == typeC;

        if (isMatch)
        {
                        
        }
        
        return isMatch;
    }
            
    
}
