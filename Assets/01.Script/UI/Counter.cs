using System;

[System.Serializable]
public class Counter
{
    public event Action<float> OnChangeValue;
    
    private float value;
    
    public float Value => value;
    
    public void Add(float value)
    {
        this.value += value;
        OnChangeValue?.Invoke(this.value);
    }
    
}