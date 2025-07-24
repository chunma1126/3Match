using UnityEngine.SceneManagement;

public class StartButton : Button
{
    private const string GAME_SCENE = "1.GameScene";
    protected override void Click()
    {
        TitleUIManager.Instance.AddEnergy(-1);
        SceneManager.LoadScene(GAME_SCENE);
    }
    
}
