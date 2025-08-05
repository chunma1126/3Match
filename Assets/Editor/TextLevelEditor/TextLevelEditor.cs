using System;
using log4net.Core;
using PlasticPipe.PlasticProtocol.Messages;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
using UIEButton = UnityEngine.UIElements.Button;


public class TextLevelEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    private static readonly Vector2Int WINDOW_SIZE = new Vector2Int(350,700);
    private static readonly Vector2Int DEFAULT_BOARD_SIZE = new Vector2Int(5,8);
    
    private const string COLOR_DATA_PREF_KEY = "TextLevelEditor_ColorDataAssetPath";
    private VisualElement panels;
    private ObjectField colorDataContainerField;
    private ObjectField levelDataField;
    private TextField textField;
    
    
    [MenuItem("Tools/TextLevelEditor")]
    public static void ShowExample()
    {
        TextLevelEditor wnd = GetWindow<TextLevelEditor>();
        wnd.titleContent = new GUIContent("TextLevelEditor");
        
        wnd.minSize = WINDOW_SIZE;
    }

    public void SetLevelData(LevelData levelData)
    {
        levelDataField.value = levelData;
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
        UIEButton createButton = root.Q<UIEButton>("CreateButton");
                
        string colorDataAssetPath = EditorPrefs.GetString(COLOR_DATA_PREF_KEY);
        if (!string.IsNullOrEmpty(colorDataAssetPath))
        {
            colorDataContainerField.value = AssetDatabase.LoadAssetAtPath<ColorDataContainer>(colorDataAssetPath);
        }
        
        createButton.clicked += HandleCreateButton(createButton);
        levelDataField.RegisterValueChangedCallback(evt => { HandleLevelDataField(evt, createButton); });
        
        textField = root.Q<TextField>("TextField");
        
        UIEButton converterButton = root.Q<UIEButton>("ConverterButton");
        converterButton.clicked += HandleConverterButton(converterButton);
    }

    private Action HandleConverterButton(UIEButton converterButton)
    {
        return () =>
        {
            if (textField.value != null && colorDataContainerField.value != null)
            {
                var converter = new TextToColorDataConverter(colorDataContainerField.value as ColorDataContainer);
                (levelDataField.value as LevelData).colorDataList=  converter.TextToColorData(textField.value);
                
                EditorUtility.SetDirty(colorDataContainerField.value);
                EditorUtility.SetDirty(levelDataField.value);
                
                AssetDatabase.SaveAssets();
            }
            
        };
    }

    private void HandleLevelDataField(ChangeEvent<Object> evt, UIEButton createButton)
    {
        if (evt.newValue != null)
        {
            panels.style.visibility = Visibility.Visible;
            panels.style.display = DisplayStyle.Flex;
                    
            createButton.style.visibility = Visibility.Hidden;
            createButton.style.display = DisplayStyle.None;
            
            Selection.activeObject = evt.newValue;
            
            LevelData levelData = evt.newValue as LevelData;
            
            var converter = new TextToColorDataConverter(colorDataContainerField.value as ColorDataContainer);
            textField.value = converter.ColorDataToText(levelData.colorDataList , levelData.boardSize.x, levelData.boardSize.y);
            
        }
        else
        {
            panels.style.visibility = Visibility.Hidden;
            panels.style.display = DisplayStyle.None;
            
            createButton.style.visibility = Visibility.Visible;
            createButton.style.display = DisplayStyle.Flex;
        }
    }

    private Action HandleCreateButton(UIEButton createButton) => () => levelDataField.value = LevelDataCreator.CreateNewLevelAsset(DEFAULT_BOARD_SIZE.x,DEFAULT_BOARD_SIZE.y);
    
}

public class TextToColorDataConverter
{
    /*string format : 
    RRRGG
    GGGYY
    BNPRR*/
    
    private ColorDataContainer colorDataContainer;
    
    public TextToColorDataConverter(ColorDataContainer colorDataContainer)
    {
        this.colorDataContainer = colorDataContainer;
    }
    
    public ColorData[] TextToColorData(string text)
    {
        string[] lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        
        int width = lines[0].Length;
        int height = lines.Length;
                                        
        ColorData[] colorDatas = new ColorData[width * height];
        
        for (int y = 0; y < height; y++)
        {
            string line = lines[y];
            for (int x = 0; x < line.Length; x++)
            {
                char color = line[x];
                ColorData currentColorData = GetColorTypeFromChar(color);
                                
                int index = y * width + x;
                colorDatas[index] = currentColorData;
            }
        }
        
        return colorDatas;
    }
        
    public string ColorDataToText(ColorData[] colorData, int width, int height)
    {
        string[] lines = new string[height];
        
        for (int y = 0; y < height; y++)
        {
            char[] lineChars = new char[width];
    
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                ColorData data = colorData[index];
                
                lineChars[x] = GetCharFromColorData(data);
            }
    
            lines[y] = new string(lineChars);
        }
    
        return string.Join("\n", lines);
    }

    private char GetCharFromColorData(ColorData data)
    {
        switch (data.ColorType)
        {
            case ColorType.Red: return 'R';
            case ColorType.Orange: return 'O';
            case ColorType.Yellow: return 'Y';
            case ColorType.Green: return 'G';
            case ColorType.Blue: return 'B';
            case ColorType.Navy: return 'N';
            case ColorType.Purple: return 'P';
            default: return '?';
        }
    }
    
    private ColorData GetColorTypeFromChar(char c)
    {
        switch (c)
        {
            case 'R': return colorDataContainer.itemList[0];
            case 'O': return colorDataContainer.itemList[1];
            case 'Y': return colorDataContainer.itemList[2];
            case 'G': return colorDataContainer.itemList[3];
            case 'B': return colorDataContainer.itemList[4];
            case 'N': return colorDataContainer.itemList[5];
            case 'P': return colorDataContainer.itemList[6];
            default: return new ColorData();
        }
    }

    
    
}
