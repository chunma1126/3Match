using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UIEButton = UnityEngine.UIElements.Button;


public class TextLevelEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    private static readonly Vector2Int WINDOW_SIZE = new Vector2Int(1200,700);
    private readonly Vector2Int DEFAULT_BOARD_SIZE = new Vector2Int(5,8);
    
    private const string COLOR_DATA_PREF_KEY = "TextLevelEditor_ColorDataAssetPath";
    private VisualElement panels;
    private ObjectField colorDataContainerField;
    private ObjectField levelDataField;
    
    
    
    [MenuItem("Tools/TextLevelEditor")]
    public static void ShowExample()
    {
        TextLevelEditor wnd = GetWindow<TextLevelEditor>();
        wnd.titleContent = new GUIContent("TextLevelEditor");
        
        wnd.minSize = WINDOW_SIZE;
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        if (colorDataContainerField != null && colorDataContainerField.value != null)
        {
            string path = AssetDatabase.GetAssetPath(colorDataContainerField.value);
            EditorPrefs.SetString(COLOR_DATA_PREF_KEY, path);
        }
    }
        
    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
                
        VisualElement uxml = m_VisualTreeAsset.Instantiate();
        uxml.style.flexGrow = 1;
        root.Add(uxml);
        
        panels = root.Q<VisualElement>("Panels");
        levelDataField = root.Q<ObjectField>("LevelDataField");
        colorDataContainerField = root.Q<ObjectField>("ColorDataContainerField");
        string colorDataAssetPath = EditorPrefs.GetString(COLOR_DATA_PREF_KEY);
        if (!string.IsNullOrEmpty(colorDataAssetPath))
        {
            colorDataContainerField.value = AssetDatabase.LoadAssetAtPath<ColorDataContainer>(colorDataAssetPath);
        }
                
        UIEButton createButton = root.Q<UIEButton>("CreateButton");
        createButton.clicked += HandleCreateButton(createButton);
        
        levelDataField.RegisterValueChangedCallback(evt =>
        {
            if (evt.newValue != null)
            {
                panels.style.visibility = Visibility.Visible;
                panels.style.display = DisplayStyle.Flex;
                    
                createButton.visible = false;
            }
            else
            {
                panels.style.visibility = Visibility.Hidden;
                panels.style.display = DisplayStyle.None;
                    
                createButton.visible = true;
            }
        });
        
    }

    private Action HandleCreateButton(UIEButton createButton) => () => levelDataField.value = LevelDataCreator.CreateNewLevelAsset(DEFAULT_BOARD_SIZE.x,DEFAULT_BOARD_SIZE.y);
    
}
