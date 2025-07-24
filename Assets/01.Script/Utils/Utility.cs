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
    
}