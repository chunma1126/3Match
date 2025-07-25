using UnityEngine;
using System.Linq;
using DG.Tweening;
using System;
using MaskTransitions;

public class Board : MonoBehaviour
{
    [SerializeField] private Vector2 startPos = new Vector2(-2,2);
    [SerializeField] private Vector2Int boardSize;
    
    [Header("Hint info")]
    [SerializeField] private float hintShowTime = 5.0f;
    private bool isShowHint = false;
    private float lastMatchTime = 0;
    
    [Header("Sound info")] 
    [SerializeField] AudioClipSO clickSound;
    [SerializeField] AudioClipSO matchSound;
        
    public int GetBoardWidthSize => boardSize.x;
    public int GetBoardHeightSize => boardSize.y;
    
    private ItemController itemController;
    private TileController tileController;
      
    private bool isMoving = false;
    private int selectFirstIndex = -1;
    private int selectSecondIndex = -1;
        
    private MatchChecker matchChecker;
    private UniqueQueue<int> itemQueue;
    private UniqueQueue<int> hintQueue;
      
    private const float SWAP_DURATION = 0.23f;

    private const string TITLE_SCENE = "0.TitleScene";
    
    private void Awake()
    {
        itemController = GetComponent<ItemController>();
        tileController = GetComponent<TileController>();
        
        tileController.Init(boardSize);
        tileController.CreateTiles(startPos);
        
        itemController.Init(tileController.Tiles);
        matchChecker = new MatchChecker(boardSize,tileController.Tiles);
                
        hintQueue = new UniqueQueue<int>(10);
        itemQueue = new UniqueQueue<int>(10);
        lastMatchTime = hintShowTime;
    }
    
    private void Start()
    {
        itemController.CreateItem();
        CheckAllTiles();
    }
    
    private void Update()
    {
        if (!isShowHint && Time.time - lastMatchTime >= hintShowTime)
        {
            ShowHint();
        }
        else if(isShowHint && Time.time - lastMatchTime < hintShowTime)
        {
            HideHint();
        }
        
        SwapProcess();
    }
        
    private void SwapProcess()
    {
        if(isMoving)return;
        
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            Tile currentTile = tileController.FindTile(Utility.GetMouseWorldPosition());

            selectFirstIndex = Array.IndexOf(tileController.TilesPositions, currentTile.transform.position);
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            Tile lastTile = tileController.FindTile(Utility.GetMouseWorldPosition());
                    
            selectSecondIndex = Array.IndexOf(tileController.TilesPositions, lastTile.transform.position);
        }
#elif UNITY_ANDROID
if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Tile currentTile = tileController.FindTile(Utility.GetTouchWorldPosition(touch.position));
                selectFirstIndex = Array.IndexOf(tileController.TilesPositions, currentTile.transform.position);
            }
            
            if (touch.phase == TouchPhase.Ended)
            {
                Tile lastTile = tileController.FindTile(Utility.GetTouchWorldPosition(touch.position));
                selectSecondIndex = Array.IndexOf(tileController.TilesPositions, lastTile.transform.position);
            }
        }
#endif
                
        bool isAdjustment = tileController.IsAdjacent(selectFirstIndex , selectSecondIndex);
        bool isTileHashFruit = tileController.HasVailedItem(selectFirstIndex , selectSecondIndex);
        
        bool canSwap = (isTileHashFruit && isAdjustment);
        if (canSwap)
        {
            TrySwap();
        }
                
    }
        
    private void TrySwap()
    {
        if (selectFirstIndex == -1 || selectSecondIndex == -1)
        {
            ResetIndex();
            return;
        }
        
        Swap(selectFirstIndex, selectSecondIndex).OnComplete(() =>
        {
            if (matchChecker.IsMatch(selectFirstIndex,selectSecondIndex,ref itemQueue))
            {
                //match
                Match();
            }
            else
            {
                //swap undo
                Swap(selectFirstIndex, selectSecondIndex).OnComplete(ResetIndex);
            }
        });
        
    }
    
    private void Match()
    {
        if(itemQueue.Count <= 0)return;
        
        UIManager.Instance.AddScore(50);
        AudioManager.Instance.PlaySound(matchSound);
        lastMatchTime = Time.time;
        
        // Swap must start from the minimum index
        itemQueue = new UniqueQueue<int>(itemQueue.OrderBy(i => i));
        
        int size = itemQueue.Count - 1;
        for (int i = 0; i <size; i++)
        {
            int index = itemQueue.Dequeue();
            tileController.RemoveItem(index);
            itemQueue.Enqueue(index); 
        }
        
        int lastIndex = itemQueue.Dequeue();
        itemQueue.Enqueue(lastIndex); 
        
        tileController.RemoveItem(lastIndex).OnComplete(() =>
        {
            int total = itemQueue.Count;
            int completed = 0;
            
            var queue = new UniqueQueue<int>(itemQueue);
            itemQueue.Clear(); 
            
            foreach (int index in queue)
            {
                ApplyGravity(index, ()=>
                {
                    completed = GravityComplete(completed, total);
                });    
            }
                        
        });
        
    }

    private int GravityComplete(int completed, int total)
    {
        ++completed;
        if (completed >= total)
        {
            completed = 0;
            itemController.RefillItem();
            CheckAllTiles();
            ResetIndex();

            bool hasNoMatch = matchChecker.FindHint().Count <= 0;
            if (hasNoMatch)
            {
                TransitionManager.Instance.LoadLevel(TITLE_SCENE);
            }
            
        }
        
        return completed;
    }

    private Tween Swap(int currentIndex,int lastIndex)
    {
        isMoving = true;
        
        Tile tileA = tileController.Tiles[currentIndex];
        Tile tileB = tileController.Tiles[lastIndex];
        
        Item itemA = tileA.CurrentItem;
        Item itemB = tileB.CurrentItem;
        
        tileA.CurrentItem = itemB;
        tileB.CurrentItem = itemA;
        
        Sequence sequence = DOTween.Sequence();
        sequence.SetAutoKill(true);
        sequence.Append(itemA.transform.DOLocalMove(Vector3.zero, SWAP_DURATION));
        sequence.Join(itemB.transform.DOLocalMove(Vector3.zero, SWAP_DURATION));
        
        return sequence;
    }
    
    private void ResetIndex()
    {
        isMoving = false;
        selectFirstIndex = -1;
        selectSecondIndex = -1;
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
        
        Swap(index, aboveIndex).OnComplete(() =>
        {
            ApplyGravity(aboveIndex,callback); 
        });
    }
    
    private void CheckAllTiles()
    {
        matchChecker.CheckAllTiles(ref itemQueue);
        Match();
    }

    private void ShowHint()
    {
        isShowHint = true;
        
        hintQueue = matchChecker.FindHint();
        foreach (var item in hintQueue)
        {
            Tile tile = tileController.Tiles[item];
            tile.CurrentItem.GetSpriteRenderer().DOKill();
            tile.CurrentItem.GetSpriteRenderer().DOFade(0.7f , 0.2f).SetLink(tile.CurrentItem.gameObject);
        }
        
    }

    private void HideHint()
    {
        isShowHint = false;
        
        foreach (var item in hintQueue)
        {
            Tile tile = tileController.Tiles[item];
            tile.CurrentItem.GetSpriteRenderer().DOKill();
            tile.CurrentItem.GetSpriteRenderer().DOFade(1, 0.2f).SetLink(tile.CurrentItem.gameObject);
        }
        hintQueue.Clear();
    }
    
    
    
}
