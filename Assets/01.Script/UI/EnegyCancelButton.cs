
public class EnegyCancelButton : Button
{
    protected override void Click()
    {
        TitleUIManager.Instance.ActiveAddPopUp(false);
    }
    
}
