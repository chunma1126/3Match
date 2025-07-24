using MaskTransitions;

public class StartButton : Button
{
    private const string GAME_SCENE = "1.GameScene";
    protected override void Click()
    {
        TitleUIManager.Instance.AddEnergy(-1);
        TransitionManager.Instance.LoadLevel(GAME_SCENE);
    }
        
}
