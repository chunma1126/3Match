using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemHandler
{
    public bool UsingItem = false;
    
    private TileController tileController;
    private BoardInput input;
    private Vector2Int boardSize;
    
    public ItemHandler(TileController tileController, BoardInput input, Vector2Int boardSize)
    {
        this.tileController = tileController;
        this.input = input;
        this.boardSize = boardSize;
    }
    
    public void TryUseItem(int index,UniqueQueue<int> itemQueue,Action<Vector3 , Vector2> callback)
    {
        var item = tileController.Tiles[index].CurrentItem;
        if (item.itemType == ItemType.Normal)
            return;
        
        if (item.itemType == ItemType.Row)
        {
            UsingItem = true;
            
            int startX = index % boardSize.x;
            
            var startPosition = tileController.Tiles[startX].transform.position;
            callback?.Invoke(startPosition,Vector2.up);
            callback?.Invoke(startPosition,Vector2.down);
            
            for (int y = 0; y < boardSize.y; y++)
            {
                int idx = startX + y * boardSize.x;
                itemQueue.Enqueue(idx);
            }
        }
        
        if (item.itemType == ItemType.Column)
        {
            UsingItem = true;
                        
            int startY = index / boardSize.x; 

            int startIdx = startY * boardSize.x;
            var startPosition = tileController.Tiles[startIdx].transform.position;
            callback?.Invoke(startPosition,Vector2.left);
            callback?.Invoke(startPosition , Vector2.right);
                        
            for (int x = 0; x < boardSize.x; x++)
            {
                int idx = startY * boardSize.x + x;
                itemQueue.Enqueue(idx);
            }
        }

        if (item.itemType == ItemType.Square)
        {
            Debug.Log("Square!!!");
        }
        
        if (item.itemType == ItemType.Five)
        {
            Debug.Log("Five!!!");
        }
                
    }
    public void TryCreateMatchItem(UniqueQueue<int> itemQueue)
    {
        if (UsingItem)
            return;
        
        var tiles = tileController.Tiles;
                
        int targetIndex = GetTargetIndex(tiles , itemQueue);
        if (targetIndex == -1)
            return;
        
        ItemType itemType = GetItemType(itemQueue);
        tiles[targetIndex].CurrentItem.SetItemType(itemType);
        
    }

    private ItemType GetItemType(UniqueQueue<int> itemQueue)
    {
        if (itemQueue == null || itemQueue.Count == 0)
            return ItemType.Normal;
    
        UniqueQueue<int> copy = new UniqueQueue<int>(itemQueue);

        if (copy.Count == 5)
            return ItemType.Five;

        if (copy.Count == 4 && IsSquare(copy))
            return ItemType.Square;

        int firstIndex = copy.Dequeue();
        int firstRow = firstIndex / boardSize.x;
        int firstCol = firstIndex % boardSize.x;
        
        bool isRowMatch = true;
        bool isColMatch = true;

        while (copy.Count > 0)
        {
            int idx = copy.Dequeue();
            int row = idx / boardSize.x;
            int col = idx % boardSize.x;

            if (row != firstRow) isRowMatch = false;
            if (col != firstCol) isColMatch = false;
        }
        
        
        if (isRowMatch) return ItemType.Row;
        if (isColMatch) return ItemType.Column;
        
        
        return ItemType.Normal;
    }

    private bool IsSquare(UniqueQueue<int> queue)
    {
        var copy = new UniqueQueue<int>(queue);
        
        HashSet<(int x, int y)> coords = new HashSet<(int x, int y)>();
        while (copy.Count > 0)
        {
            int idx = copy.Dequeue();
            int y = idx / boardSize.x;
            int x = idx % boardSize.x;
            coords.Add((x, y));
        }
        
        if (coords.Count != 4)
            return false;

        int minX = coords.Min(p => p.x);
        int maxX = coords.Max(p => p.x);
        int minY = coords.Min(p => p.y);
        int maxY = coords.Max(p => p.y);
        
        return (maxX - minX == 1) &&
               (maxY - minY == 1);
    }
    
    private int GetTargetIndex(Tile[] tiles,UniqueQueue<int> itemQueue)
    {
        if (IsNormalItem(tiles, input.SelectFirstIndex) && itemQueue.TryRemove(input.SelectFirstIndex))
            return input.SelectFirstIndex;
        
        if (IsNormalItem(tiles,input.SelectSecondIndex) && itemQueue.TryRemove(input.SelectSecondIndex))
            return input.SelectSecondIndex;
        
        int peekIndex = itemQueue.Peek();
        if (IsNormalItem(tiles, peekIndex))
            return peekIndex;
        
        return -1;
    }
    private bool IsNormalItem(Tile[] tiles, int index)
    {
        return index != -1 && tiles[index].CurrentItem?.itemType == ItemType.Normal;
    }
}