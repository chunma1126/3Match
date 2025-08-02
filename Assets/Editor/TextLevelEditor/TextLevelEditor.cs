using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class TextLevelEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    private static readonly Vector2Int WINDOW_SIZE = new Vector2Int(1200,700);
    
    [MenuItem("Tools/TextLevelEditor")]
    public static void ShowExample()
    {
        TextLevelEditor wnd = GetWindow<TextLevelEditor>();
        wnd.titleContent = new GUIContent("TextLevelEditor");

        wnd.minSize = WINDOW_SIZE;
    }
    
    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
                
        VisualElement uxml = m_VisualTreeAsset.Instantiate();
        uxml.style.flexGrow = 1;
        root.Add(uxml);

        /*var objectField = new ObjectField();
        objectField.objectType = typeof(ColorDataContainer);
        objectField.style.flexGrow = 1;
        objectField.style.flexShrink = 1;
        root.Add(objectField);*/
        
    }
}
