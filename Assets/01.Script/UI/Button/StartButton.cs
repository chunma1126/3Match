using MaskTransitions;

public class StartButton : Button
{
    private const string GAME_SCENE = "1.GameScene";
    protected override void Click()
    {
        if (GameManager.Instance.HasEnergy())
        {
            GameManager.Instance.energyCounter.Add(-1);
            TransitionManager.Instance.LoadLevel(GAME_SCENE);
        }
        else
        {
           PopupManager.Instance.PopUp(PopupType.Add);
        }
        
    }
        
}
