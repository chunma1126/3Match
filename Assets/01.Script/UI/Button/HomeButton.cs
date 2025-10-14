
public class HomeButton : SceneTransitionButton
{
    protected override void Click()
    {
        GameManager.Inst.SaveScore();
        GameManager.Inst.InitScore();
        
        base.Click();
    }
                
}
