using System.Linq;
using DG.Tweening;
using UnityEngine;

public class TileController : MonoBehaviour
{
    [Header("Prefab info")]
    [SerializeField] private Tile[] tilePrefabs;

    public Tile[] Tiles => tiles;
    public Vector2[] TilesPositions => tilePositions;
        
    private Tile[] tiles;
    private Vector2[] tilePositions;
    
    private Vector2Int boardSize;
    
    private const int TILE_SIZE_WIDTH = 1;
    private const int TILE_SIZE_HEIGHT = 1;
    
    public void Init(Vector2Int boardSize)
    {
        this.boardSize = boardSize;
    }
    
    public void CreateTiles(Vector2 startPos)
    {
        tiles = new Tile[boardSize.x * boardSize.y];
        tilePositions = new Vector2[boardSize.x * boardSize.y];
        
        for (int x = 0; x < boardSize.x; x++)
        {
            for (int y = 0; y < boardSize.y; y++)
            {
                Vector2 pos = startPos + new Vector2(x * TILE_SIZE_WIDTH, -y * TILE_SIZE_HEIGHT);
                Tile tile = Instantiate(tilePrefabs[(y + x) % 2], pos, Quaternion.identity);
                tile.transform.SetParent(transform);
                
                tile.gameObject.name = $"Tile_{x}_{y}";
                
                tiles[GetIndex(x,y)] = tile;
                tilePositions[GetIndex(x,y)] = tile.transform.position;
            }
        }
        int GetIndex(int x, int y)
        {
            return y * boardSize.x + x;
        }
    }
    
    public Tile FindTile(Vector2 mousePos)
    {
        return tiles
            .OrderBy(tile => Vector2.Distance(mousePos, tile.transform.position))
            .FirstOrDefault()
            ?.GetComponentInChildren<Tile>();
    }
    
    public bool IsAdjacent(int index1, int index2)
    {
        Vector2Int pos1 = IndexToPosition(index1);
        Vector2Int pos2 = IndexToPosition(index2);
    
        int manhattanDistance = Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y);
        return manhattanDistance == 1;
    }

    public bool HasVailedItem(int selectFirstIndex , int selectSecondIndex)
    {
        return selectFirstIndex != -1 && selectSecondIndex != -1 &&
            tiles[selectFirstIndex].CurrentItem.colorData.colorType != ColorType.None &&
            tiles[selectSecondIndex].CurrentItem.colorData.colorType != ColorType.None;
    }

    public Tween RemoveItem(int index)
    {
        ColorData colorData = new ColorData();
        colorData.colorType = ColorType.None;
           
        return tiles[index].CurrentItem.SetData(colorData);
    }
        
    private Vector2Int IndexToPosition(int index)
    {
        return new Vector2Int(index % boardSize.x, index / boardSize.x);
    }
    
    
    
}