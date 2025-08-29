using UnityEngine;

public class BombButton : Button
{
    
    protected override void Click()
    {
        if (GameManager.Inst.bombCounter.Value <= 0)
        {
            return;
        }
        GameManager.Inst.bombCounter.Add(-1);
        
        
        
    }

    
}
