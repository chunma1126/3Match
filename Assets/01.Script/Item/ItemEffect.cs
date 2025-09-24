using System;
using UnityEngine;

public class ItemEffect : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    
    private Vector2 direction;
    
    public void SetDirection(Vector2 direction)
    {
        this.direction = direction;
    }
    
    private void Update()
    {
        transform.position += new Vector3(direction.x , direction.y , 0.0f) * (moveSpeed * Time.deltaTime);        
    }
    
}
