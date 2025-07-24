using UnityEngine;

public static class Utility
{
    /// <summary>
    /// 현재 마우스 위치를 2D 월드 좌표로 반환합니다.
    /// </summary>
    public static Vector2 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
    
    public static Vector3 GetTouchWorldPosition(Vector2 screenPosition)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPosition);
        worldPos.z = 0f; // 2D 게임이라면 Z값 고정
        return worldPos;
    }
    
}