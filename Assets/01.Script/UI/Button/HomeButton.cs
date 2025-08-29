
public class HomeButton : SceneTransitionButton
{
    protected override void Click()
    {
        base.Click();
        
        GameManager.Inst.SaveScore();
        GameManager.Inst.InitScore();
    }
        
}
