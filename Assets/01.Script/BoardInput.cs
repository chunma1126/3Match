using System;
using UnityEngine;

public class BoardInput
{
    public bool CanInput = true;
    
    private int selectFirstIndex = -1;
    private int selectSecondIndex = -1;
    
    public int SelectFirstIndex => selectFirstIndex;
    public int SelectSecondIndex => selectSecondIndex;
    
    public bool HasValue => selectFirstIndex != -1 && selectSecondIndex != -1;
    
    private readonly TileController tileController;
    private Vector3 currentMousePosition;
    
    public BoardInput(TileController tileController)
    {
        this.tileController = tileController;
    }
    
    public void Update()
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
        
        //Debug.Log(selectFirstIndex + "  " + selectSecondIndex);
        
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
    }
        
    public void ResetIndex()
    {
        selectFirstIndex = -1;
        selectSecondIndex = -1;
    }

    public bool IsAdjustment()
    {
        return tileController.IsAdjacent(selectFirstIndex , selectSecondIndex);
    }

    public bool HasVailedItem()
    {
        return tileController.HasVailedItem(selectFirstIndex ,selectSecondIndex);
    }
    
    
}