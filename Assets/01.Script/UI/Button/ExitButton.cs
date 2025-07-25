using UnityEngine;

public class ExitButton : Button
{
    protected override void Click()
    {
        Application.Quit();
    }
}
