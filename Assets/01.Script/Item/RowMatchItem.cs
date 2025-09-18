using System.Runtime.CompilerServices;
using UnityEngine;

public class RowMatchItem : IActivatable
{
    private int _index;
    private Vector2Int _boardSize;
    public RowMatchItem(int index , Vector2Int boardSize)
    {
        _index = index;
        _boardSize = boardSize;
    }
    public void Execute()
    {
        int x = _index % _boardSize.x;
        
                
    }
    
}
