using UnityEngine;
using System.Linq;
using DG.Tweening;
using System;

public class Board : MonoBehaviour
{
    [SerializeField] private Vector2 startPos = new Vector2(-2,2);
    [SerializeField] private Vector2Int boardSize;
    
    [Header("Hint info")]
    [SerializeField] private float hintShowTime = 5.0f;
    private bool isShowHint = false;
    private float lastMatchTime = 0;
    
    [Header("Sound info")]
    [SerializeField] private AudioClipSO matchSound;
        
    public int GetBoardWidthSize => boardSize.x;
    public int GetBoardHeightSize => boardSize.y;
    
    private ItemController itemController;
    private TileController tileController;
      
    private bool canInput = true;
    private int selectFirstIndex = -1;
    private int selectSecondIndex = -1;
        
    private MatchChecker matchChecker;
    private UniqueQueue<int> itemQueue;
    private UniqueQueue<int> hintQueue;
      
    private const float SWAP_DURATION = 0.23f;
    
    private Vector3 currentMousePosition;
        
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
        itemController.CreateItem().OnComplete(() =>
        {
            bool hasNoMatch = matchChecker.FindHint().Count <= 0;
        
            if (hasNoMatch)
            {
                ReRollBoard();
            }
            else
            {
                CheckAllTiles();
            }
            
            canInput = true;        
        });
                
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
        
        if(canInput)
            SwapProcess();
    }
    
    private void SwapProcess()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        
        if (Input.GetMouseButtonDown(0))
        {
            currentMousePosition = Utility.GetMouseWorldPosition();
            Tile currentTile = tileController.FindTile(currentMousePosition);
            int currentIndex = Array.IndexOf(tileController.TilesPositions, currentTile.transform.position);
    
            selectFirstIndex = currentIndex;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 newMousePosition = Utility.GetMouseWorldPosition();
            if (Vector3.Distance(currentMousePosition, newMousePosition) > 0.1f) 
            {
                Tile currentTile = tileController.FindTile(newMousePosition); 
                if (currentTile != null)
                {
                    int currentIndex = Array.IndexOf(tileController.TilesPositions, currentTile.transform.position);
                    if(selectFirstIndex != currentIndex)
                        selectSecondIndex = currentIndex;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            ResetIndex();
        }
        
#elif UNITY_ANDROID
if (Input.touchCount > 0)
{
    Touch touch = Input.GetTouch(0);

    if (touch.phase == TouchPhase.Began)
    {
        Vector3 touchWorldPos = Utility.GetTouchWorldPosition(touch.position);
        Tile currentTile = tileController.FindTile(touchWorldPos);
        int currentIndex = Array.IndexOf(tileController.TilesPositions, currentTile.transform.position);
        
        selectFirstIndex = currentIndex;
        currentMousePosition = touchWorldPos; // 시작 터치 위치 저장
    }
    
    if (touch.phase == TouchPhase.Moved)
    {
        Vector3 currentTouchPos = Utility.GetTouchWorldPosition(touch.position);
        if (Vector3.Distance(currentMousePosition, currentTouchPos) > 0.1f)
        {
            Tile currentTile = tileController.FindTile(currentTouchPos);
            if (currentTile != null)
            {
                int currentIndex = Array.IndexOf(tileController.TilesPositions, currentTile.transform.position);
                if(selectFirstIndex != currentIndex)
                    selectSecondIndex = currentIndex;
            }
        }
    }
    
    if (touch.phase == TouchPhase.Ended)
    {
        ResetIndex();
    }
}
#endif
                
        bool isAdjustment = tileController.IsAdjacent(selectFirstIndex , selectSecondIndex);
        bool isTileHashFruit = tileController.HasVailedItem(selectFirstIndex , selectSecondIndex);
        bool canSwap = (isTileHashFruit && isAdjustment);
        
        if (canSwap)
        {
            canInput = false;
            
            TrySwap();
        }
        
    }
        
    private void TrySwap()
    {
        if (selectFirstIndex == -1 || selectSecondIndex == -1 || !GameManager.Instance.HasMoveCount)
        {
            if (!GameManager.Instance.HasMoveCount)
            {
                PopupManager.Instance.PopUp(PopupType.Add);
            }
            
            canInput = true;
            ResetIndex();
            return;
        }
        
        Swap(selectFirstIndex, selectSecondIndex).OnComplete(() =>
        {
            bool match = matchChecker.IsMatch(selectFirstIndex, selectSecondIndex, ref itemQueue);
            if (match)
            {
                GameManager.Instance.moveCounter.Add(-1);
                Match();
            }
            else
            {
                //swap undo
                Swap(selectFirstIndex, selectSecondIndex).OnComplete(() =>
                {
                    ResetIndex();
                    canInput = true;
                });
            }
        });
        
    }
    
    private Tween Swap(int currentIndex,int lastIndex)
    {
        canInput = false;
        
        Tile tileA = tileController.Tiles[currentIndex];
        Tile tileB = tileController.Tiles[lastIndex];
        
        Item itemA = tileA.CurrentItem;
        Item itemB = tileB.CurrentItem;
        
        tileA.CurrentItem = itemB;
        tileB.CurrentItem = itemA;
        
        Sequence sequence = DOTween.Sequence();
        sequence.Append(itemA.transform.DOLocalMove(Vector3.zero, SWAP_DURATION).SetLink(itemA.gameObject));
        sequence.Join(itemB.transform.DOLocalMove(Vector3.zero, SWAP_DURATION).SetLink(itemB.gameObject));
        
        return sequence;
    }
        
    private void Match()
    {
        if (itemQueue.Count <= 0)
        {
            canInput = true;
            Debug.Log("item queue is empty");
            return;
        }
        
        GameManager.Instance.AddScore(50);
        AudioManager.Instance.PlaySound(matchSound);
        lastMatchTime = Time.time;
        
            
        // Swap must start from the minimum index
        itemQueue = new UniqueQueue<int>(itemQueue.OrderBy(i => i));
        
        int size = itemQueue.Count - 1;
        for (int i = 0; i < size; i++)
        {
            int index = itemQueue.Dequeue();
            tileController.RemoveItem(index);
            itemQueue.Enqueue(index); 
        }
        
        int lastIndex = itemQueue.Dequeue();
        itemQueue.Enqueue(lastIndex); 
        
        tileController.RemoveItem(lastIndex).OnComplete(() =>
        {
            var queue = new UniqueQueue<int>(itemQueue);
                        
            itemQueue.Clear(); 
            int total = queue.Count;
            int completed = 0;
            
                        
            foreach (int index in queue)
            {
                ApplyGravity(index, ()=>
                {
                    completed = GravityComplete(completed, total);
                });    
            }
            
        });
        
    }
    
        
    private void ApplyGravity(int index,Action callback = null)
    {
        int aboveIndex = index - GetBoardWidthSize;
        
        if (aboveIndex < 0)
        {
            callback?.Invoke();
            return;
        }
                
        Swap(index, aboveIndex).OnComplete(() =>
        {
            ApplyGravity(aboveIndex,callback); 
        });
    }
    
    private int GravityComplete(int completed, int total)
    {
        ++completed;
        if (completed >= total)
        {
            completed = 0;
            itemController.RefillItem().OnComplete(() =>
            {
                bool hasNoMatch = matchChecker.FindHint().Count <= 0;
                
                if (hasNoMatch)
                {
                    Invoke(nameof(ReRollBoard) , 2f);
                }
                else
                {
                    CheckAllTiles();
                    ResetIndex();
                }
            });
                        
        }
        
        return completed;
    }
    
    private void CheckAllTiles()
    {
        matchChecker.CheckAllTiles(ref itemQueue);
        Match();
    }
    
    private void ResetIndex()
    {
        selectFirstIndex = -1;
        selectSecondIndex = -1;
    }
    
    [ContextMenu("ReRoll Board")]
    public void ReRollBoard()
    {
        itemController.ReRollItem().OnComplete(CheckAllTiles);
    }
    
    #region Hint
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
    #endregion
    
}
