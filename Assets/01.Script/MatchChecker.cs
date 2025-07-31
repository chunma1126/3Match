using System.Collections.Generic;
using UnityEngine;

public class MatchChecker
{
    private readonly Tile[] tiles;
    private readonly Vector2Int boardSize;

    private const int MATCH_MIN_SIZE = 3;
    
    public MatchChecker(Vector2Int boardSize,Tile[] tiles)
    {
        this.boardSize = boardSize;
        this.tiles = tiles;
    }

    public void CheckAllTiles(ref UniqueQueue<int> queue)
    {
        for (int y = 0; y < boardSize.y; y++)
        {
            for (int x = 0; x < boardSize.x; x++)
            {
                int index = y * boardSize.x + x;
                CheckHorizontal(index, ref queue);
                CheckVertical(index, ref queue);
                
            }
        }
        
    }
    
    public bool IsMatch(int currentFruitIndex , int lastFruitIndex, ref UniqueQueue<int> queue)
    {
        bool checkHorizontal1 = CheckHorizontal(currentFruitIndex, ref queue);
        bool checkHorizontal2 = CheckHorizontal(lastFruitIndex,ref queue);
        
        bool checkVertical1 = CheckVertical(currentFruitIndex,ref queue);
        bool checkVertical2 = CheckVertical(lastFruitIndex, ref queue);
        
        return checkHorizontal1 | checkHorizontal2 | checkVertical1 | checkVertical2;
    }

    public UniqueQueue<int> FindHint()
    {
        UniqueQueue<int> hintQueue = new UniqueQueue<int>();

        for (int y = 0; y < boardSize.y; y++)
        {
            for (int x = 0; x < boardSize.x; x++)
            {
                int index = y * boardSize.x + x;
                
                if (x + 1 < boardSize.x)
                {
                    int right = index + 1;
                    (tiles[index], tiles[right]) = (tiles[right], tiles[index]);

                    UniqueQueue<int> tempQueue = new UniqueQueue<int>();
                    bool hasMatch = CheckHorizontal(index, ref tempQueue) || 
                                   CheckVertical(index, ref tempQueue) ||
                                   CheckHorizontal(right, ref tempQueue) || 
                                   CheckVertical(right, ref tempQueue);

                    if (hasMatch)
                    {
                        hintQueue.Enqueue(index);
                        hintQueue.Enqueue(right);
                    }

                    (tiles[index], tiles[right]) = (tiles[right], tiles[index]);
                    
                    if (hintQueue.Count > 0)
                        return hintQueue;
                }
                
                if (y + 1 < boardSize.y)
                {
                    int down = index + boardSize.x;
                    (tiles[index], tiles[down]) = (tiles[down], tiles[index]);

                    UniqueQueue<int> tempQueue = new UniqueQueue<int>();
                    bool hasMatch = CheckHorizontal(index, ref tempQueue) || 
                                   CheckVertical(index, ref tempQueue) ||
                                   CheckHorizontal(down, ref tempQueue) || 
                                   CheckVertical(down, ref tempQueue);

                    if (hasMatch)
                    {
                        hintQueue.Enqueue(index);
                        hintQueue.Enqueue(down);
                    }

                    (tiles[index], tiles[down]) = (tiles[down], tiles[index]);
                    
                    if (hintQueue.Count > 0)
                        return hintQueue;
                }
            }
        }
        
        return hintQueue;
    }
    
    private bool CheckHorizontal(int index , ref UniqueQueue<int> queue)
    {
        if (tiles[index].CurrentItem.colorData.ColorType == ColorType.None) return false;
        
        int width = boardSize.x; 
        int x = index % width;
        
        // left (x-2, x-1, x)
        if (x >= 2)
        {
            int i1 = index - 2;
            int i2 = index - 1;
            int i3 = index;
            IsTripleMatch(i1, i2, i3, ref queue);
        }
        
        // center (x-1, x, x+1)
        if (x >= 1 && x + 1 < width)
        {
            int i1 = index - 1;
            int i2 = index;
            int i3 = index + 1;
            IsTripleMatch(i1, i2, i3, ref queue);
        }

        // right (x, x+1, x+2)
        if (x + 2 < width)
        {
            int i1 = index;
            int i2 = index + 1;
            int i3 = index + 2;
            IsTripleMatch(i1, i2, i3, ref queue);
        }
        
        return queue.Count >= MATCH_MIN_SIZE;
    }
    
    private bool CheckVertical(int index, ref UniqueQueue<int> queue)
    {
        if (tiles[index].CurrentItem.colorData.ColorType == ColorType.None) return false;
        
        int width = boardSize.x;
        int height = boardSize.y;
        
        int y = index / width;
        
        //up
        if (y >= 2)
        {
            int currentY = index;
            int up1 = currentY - boardSize.x;
            int up2 = currentY -  boardSize.x * 2;
            
            IsTripleMatch(currentY, up1, up2, ref queue);
        }
        
        //center
        if (y >= 1 && y + 1 < height)
        {
            int currentX = index;
            int up = currentX - boardSize.x;
            int down = currentX +  boardSize.x;
            
            IsTripleMatch(currentX, up, down, ref queue);
        }
        
        //Right
        if (y + 2 < height)
        {
            int currentX = index;
            int down = currentX +  boardSize.x;
            int down2 = currentX +  boardSize.x * 2;

            IsTripleMatch(currentX, down, down2, ref queue);
        }
        return queue.Count >= MATCH_MIN_SIZE;
    }
    
    private bool IsTripleMatch(int a, int b, int c, ref UniqueQueue<int> queue)
    {
        int maxWidth = tiles.Length;
        if (a < 0 || b < 0 || c < 0 || a >= maxWidth || b >= maxWidth || c >= maxWidth)
            return false;
        
        var typeA = tiles[a].CurrentItem.colorData.ColorType;
        var typeB = tiles[b].CurrentItem.colorData.ColorType;
        var typeC = tiles[c].CurrentItem.colorData.ColorType;
        
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