using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;




public class Board : MonoBehaviour
{
    [SerializeField] private Vector2 startPos = new Vector2(-2,2);
    [SerializeField] private Vector2Int boardSize;
    
    [Range(0,1)][SerializeField] private float swapDuration = 0.23f;
    [SerializeField] private AssetReferenceGameObject itemEffect;
    
    [Header("Hint info")]
    [SerializeField] private float hintShowTime = 5.0f;
    private bool isShowHint = false;
    private float lastMatchTime = 0;
    
    [Header("Sound info")]
    [SerializeField] private AssetReferenceAudioSO matchSound;
    
    private ItemController itemController;
    private TileController tileController;
    private BoardInput input;
    private ItemHandler itemHandler;
    
    private MatchChecker matchChecker;
    private UniqueQueue<int> itemQueue;
    private UniqueQueue<int> hintQueue;
    
  
    private const float REROLL_TIME = 0.8f;
        
    private void Awake()
    {
        itemController = GetComponent<ItemController>();
        tileController = GetComponent<TileController>();
        
        tileController.Init(boardSize);
        tileController.CreateTiles(startPos);
        
        itemController.Init(tileController.Tiles);
        
        matchChecker = new MatchChecker(boardSize,tileController.Tiles);
        input = new  BoardInput(tileController);
        itemHandler = new ItemHandler(tileController , input , boardSize);
        
        hintQueue = new UniqueQueue<int>(10);
        itemQueue = new UniqueQueue<int>(10);
        lastMatchTime = hintShowTime;
    }
    
    private async void Start()
    {
        await itemController.CreateItem(); 
        
        bool hasNoMatch = matchChecker.FindMatch().Count <= 0;
        
        if (hasNoMatch)
        {
            ReRollBoard();
        }
        else
        {
            CheckAllTiles();
        }
            
        input.CanInput = true;        
                
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

        if (input.CanInput)
        {
            input.Update();

            if (input.HasValue)
            {
                SwapProcess();
            }
            
        }
    }
    
    private void SwapProcess()
    {
        bool isAdjustment = input.IsAdjustment();
        bool isTileHashFruit = input.HasVailedItem();
        bool canSwap = (isTileHashFruit && isAdjustment);
        
        //Debug.Log($"isAdjustment : {isAdjustment}, isTileHashFruit : {isTileHashFruit}, canSwap : {canSwap}");
        
        if (canSwap)
        {
            input.CanInput = false;
            
            TrySwap();
        }
        
    }
        
    private void TrySwap()
    {
        if (!input.HasValue || !GameManager.Inst.HasMoveCount)
        {
            if (!GameManager.Inst.HasMoveCount)
            {
                PopupManager.Inst.PopUp(PopupType.Add);
            }
            
            input.CanInput = true;
            input.ResetIndex();
            return;
        }
        
        Swap(input.SelectFirstIndex, input.SelectSecondIndex).OnComplete(() =>
        {
            bool match = matchChecker.IsMatch(input.SelectFirstIndex, input.SelectSecondIndex, ref itemQueue);
            if (match)
            {
                GameManager.Inst.moveCounter.Add(-1);
                
                Match();
            }
            else
            {
                //swap undo
                Swap(input.SelectFirstIndex, input.SelectSecondIndex).OnComplete(() =>
                {
                    input.ResetIndex();
                    input.CanInput = true;
                });
            }
        });
        
    }
    
    private Tween Swap(int currentIndex,int lastIndex)
    {
        input.CanInput = false;
        
        Tile tileA = tileController.Tiles[currentIndex];
        Tile tileB = tileController.Tiles[lastIndex];
        
        Item itemA = tileA.CurrentItem;
        Item itemB = tileB.CurrentItem;
        
        tileA.CurrentItem = itemB;
        tileB.CurrentItem = itemA;
        
        Sequence sequence = DOTween.Sequence();
        sequence.Append(itemA.transform.DOLocalMove(Vector3.zero, swapDuration).SetLink(itemA.gameObject));
        sequence.Join(itemB.transform.DOLocalMove(Vector3.zero, swapDuration).SetLink(itemB.gameObject));
        
        return sequence;
    }
    
    private void Match()
    {
        if (itemQueue.Count <= 0)
        {
            input.CanInput = true;
            //Debug.Log("item queue is empty");
            return;
        }
        
        var copy = new List<int>(itemQueue);
        foreach (var item in copy)
        {
            itemHandler.TryUseItem(item,itemQueue , CreateItemEffect);
        }
        
        if(itemQueue.Count > 3 && IsAllSameColor(itemQueue))
            itemHandler.TryCreateMatchItem(itemQueue);
        
        MatchProcess();
        
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

    private async Task MatchProcess()
    {
        GameManager.Inst.AddScore(25 * itemQueue.Count);
        
        var mathSound = await AssetManager.LoadAsync<AudioSO>(matchSound);
        AudioManager.Inst.PlaySound(mathSound);
        
        lastMatchTime = Time.time;
    }

    private async void CreateItemEffect(Vector3 position, Vector2 dir)
    {
        ItemEffect obj = await AssetManager.LoadAsync<ItemEffect>(itemEffect);
        obj.SetDirection(dir);
        
        //to do : change to pool
        Destroy(obj, 3f);
    }
    
    private void ApplyGravity(int index,Action callback = null)
    {
        int aboveIndex = index - boardSize.x;
        
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
            itemHandler.UsingItem = false;
            
            completed = 0;
            itemController.RefillItem().Result.OnComplete(() =>
            {
                bool hasNoMatch = matchChecker.FindMatch().Count <= 0;
              
                if (hasNoMatch)
                {
                    Invoke(nameof(ReRollBoard) , REROLL_TIME);
                }
                else
                {
                    CheckAllTiles();
                    input.ResetIndex();
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
        
    private bool IsAllSameColor(UniqueQueue<int> queue)
    {
        if (queue.Count == 0)
            return false; 
        
        UniqueQueue<int> q = new UniqueQueue<int>(queue);

        int firstIndex = q.Dequeue();
        ColorData firstColor = tileController.Tiles[firstIndex].CurrentItem.colorData;
        
        while (q.Count > 0)
        {
            int idx = q.Dequeue();
            if (tileController.Tiles[idx].CurrentItem.colorData.NotEquals(firstColor))
                return false; 
        }
        
        return true; 
    }
    
    
        
    #region Hint
    private void ShowHint()
    {
        isShowHint = true;
        
        hintQueue = matchChecker.FindMatch();
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
    
    [ContextMenu("ReRoll Board")]
    public void ReRollBoard()
    {
        input.CanInput = false;
        itemController.ReRollItem().OnComplete(()=>
        {
            input.ResetIndex();
            CheckAllTiles();
        });
    }
    
}
