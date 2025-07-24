using UnityEngine;

public abstract class Button : MonoBehaviour
{
    private void Start()
    {
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(Click);
    }

    private void OnDestroy()
    {
        GetComponent<UnityEngine.UI.Button>().onClick.RemoveListener(Click);
    }
    
    protected abstract void Click();
}