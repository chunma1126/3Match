using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UniqueQueue<T> : IEnumerable<T> where T : IEquatable<T>
{
    private Queue<T> queue = new Queue<T>();
    private HashSet<T> set = new HashSet<T>();
       
    public int Count => queue.Count;
    
    public UniqueQueue()
    {
        
    }
    
    public UniqueQueue(int value)
    {
        queue = new Queue<T>(value);
        set = new HashSet<T>(value);
    }
    
    public UniqueQueue(IEnumerable<T> collection)
    {
        foreach (var item in collection)
        {
            Enqueue(item);
        }
    }
    
    public bool Enqueue(T item)
    {
        if (set.Contains(item))
            return false;

        queue.Enqueue(item);
        set.Add(item);
        return true;
    }
    
    public T Dequeue()
    {
        var item = queue.Dequeue();
        set.Remove(item);
        return item;
    }
    
    public T Peek()
    {
        if (queue.Count == 0 && set.Count == 0)
        {
            return default(T);
        }
        
        return queue.Peek();
    }
    
    public bool TryRemove(T value)
    {
        if (!set.Contains(value))
        {
            //Debug.LogWarning($"UniqueQueue Remove Failed: {value} does not exist in the queue.");
            return false;
        }
        else
        {
            Remove(value);
            return true;
        }
        
    }
    
    
    private void Remove(T value) 
    {
        set.Remove(value);
        
        int count = queue.Count;
        Queue<T> newQueue = new Queue<T>();
        
        for (int i = 0; i < count; i++)
        {
            var item = queue.Dequeue();
            if (!item.Equals(value)) 
                newQueue.Enqueue(item);
            else
                break; 
        }

        while (queue.Count > 0)
            newQueue.Enqueue(queue.Dequeue());

        queue = newQueue;
    }

    
    public void Clear()
    {
        queue.Clear();
        set.Clear();
    }
    
    public bool Contains(T item) => set.Contains(item);

    public IEnumerator<T> GetEnumerator() => queue.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}