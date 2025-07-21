using System.Collections;
using System.Collections.Generic;

public class UniqueQueue<T> : IEnumerable<T>
{
    private Queue<T> queue = new Queue<T>();
    private HashSet<T> set = new HashSet<T>();

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

    public int Count => queue.Count;

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

    public void Clear()
    {
        queue.Clear();
        set.Clear();
    }

    public bool Contains(T item) => set.Contains(item);

    public IEnumerator<T> GetEnumerator() => queue.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}