using UnityEngine.SceneManagement;

public class StartButton : Button
{
    protected override void Click()
    {
        TitleUIManager.Instance.AddEnergy(-1);
        SceneManager.LoadScene("1.GameScene");
    }
    
}
