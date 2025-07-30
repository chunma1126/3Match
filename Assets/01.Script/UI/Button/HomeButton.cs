
public class HomeButton : SceneTransitionButton
{
    protected override void Click()
    {
        base.Click();
        
        GameManager.Instance.SaveScore();
        GameManager.Instance.InitScore();
    }
        
}
