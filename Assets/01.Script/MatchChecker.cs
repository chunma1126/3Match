
using System.Collections.Generic;
using UnityEngine;

public class MatchChecker
{
    private readonly Tile[] tiles;
    private readonly Vector2Int boardSize;
    
    public MatchChecker(Vector2Int boardSize,Tile[] tiles)
    {
        this.boardSize = boardSize;
        this.tiles = tiles;
    }
    
    public bool Match(int currentFruitIndex , int lastFruitIndex, ref Queue<int> queue)
    {
        if (CheckHorizontal(currentFruitIndex,ref queue)) return true;
        if (CheckHorizontal(lastFruitIndex,ref queue)) return true;
        
        if (CheckVertical(currentFruitIndex,ref queue)) return true;
        if (CheckVertical(lastFruitIndex,ref queue)) return true;
        
        return false;
    }
    
    private bool CheckHorizontal(int index , ref Queue<int> queue)
    {
        //left
        {
            int currentX = index;
            int left = currentX - 1;
            int left2 = currentX - 2;
            
            if (IsTripleMatch(currentX , left ,left2,ref queue))
            {
                return true;
            }
        }
        
        //center
        {
            int currentX = index;
            int left = currentX - 1;
            int right = currentX + 1;
            
            if (IsTripleMatch(currentX , left ,right,ref queue))
            {
                return true;
            }
        }
        
        //Right
        {
            int currentX = index;
            int right1 = currentX + 1;
            int right2 = currentX + 2;
            
            if (IsTripleMatch(currentX , right1 ,right2,ref queue))
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckVertical(int index, ref Queue<int> queue)
    {
        //up
        {
            int currentY = index;
            int up1 = currentY - boardSize.x;
            int up2 = currentY -  boardSize.x  * 2;
            
            if (IsTripleMatch(currentY , up1 ,up2,ref queue))
            {
                return true;
            }
        }
        
        //center
        {
            int currentX = index;
            int up = currentX - boardSize.x;
            int down = currentX +  boardSize.x;
            
            if (IsTripleMatch(currentX , up ,down,ref queue))
            {
                return true;
            }
        }
        
        //Right
        {
            int currentX = index;
            int down = currentX +  boardSize.x;
            int down2 = currentX +  boardSize.x * 2;
            
            if (IsTripleMatch(currentX , down ,down2,ref queue))
            {
                return true;
            }
        }
        return false;
    }
    
    private bool IsTripleMatch(int a, int b, int c, ref Queue<int> queue)
    {
        int maxWidth = tiles.Length;
        if (a < 0 || b < 0 || c < 0 || a >= maxWidth || b >= maxWidth || c >= maxWidth)
            return false;
        
        var typeA = tiles[a].CurrentFruit.fruitData.fruitType;
        var typeB = tiles[b].CurrentFruit.fruitData.fruitType;
        var typeC = tiles[c].CurrentFruit.fruitData.fruitType;
        
        bool isMatch = typeA == typeB && typeA == typeC;
        
        if (isMatch)
        {
            queue.Enqueue(a);
            queue.Enqueue(b);
            queue.Enqueue(c);
        }
        
        return isMatch;
    }
    
}