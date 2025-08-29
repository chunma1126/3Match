using UnityEngine;

public class VolumeTestButton : Button
{
    [SerializeField] private AudioSO audioClip;
    protected override void Click()
    {
        AudioManager.Inst.PlaySound(audioClip);    
    }
        
}
