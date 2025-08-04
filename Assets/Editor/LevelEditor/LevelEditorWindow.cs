using UnityEditor;
using UnityEngine;

public class LevelEditorWindow : EditorWindow
{
    private int width = 5;
    private int height = 5;
    
    private ColorDataContainer colorDataContainer;
    private LevelData levelData;
    
    private ColorData[] currentColorDataList;
    
    private Vector2 scrollPos;
    private static readonly Vector2Int WINDOW_SIZE = new Vector2Int(500, 800);
    
    #region Const Strings
    private const string GRID_WIDTH = "Grid_Width";
    private const string GRID_HEIGHT = "Grid_Height";
    private const string COLOR_DATA_CONTAINER = "Color_Data_Container";
    #endregion
        
    [MenuItem("Tools/Level Editor")]
    public static void ShowWindow()
    {
        var window = GetWindow<LevelEditorWindow>("LevelEditor");
        window.minSize = WINDOW_SIZE;
    }
    
    public void SetLevelData(LevelData data)
    {
        levelData = data;
        currentColorDataList = data.Copy();
    }
    
    private void OnEnable()
    {
        width = EditorPrefs.GetInt(GRID_WIDTH, 5);
        height = EditorPrefs.GetInt(GRID_HEIGHT, 8);
        
        string colorDataContainerPath = EditorPrefs.GetString(COLOR_DATA_CONTAINER, "");
        if (!string.IsNullOrEmpty(colorDataContainerPath))
        {
            colorDataContainer = AssetDatabase.LoadAssetAtPath<ColorDataContainer>(colorDataContainerPath);
        }
        
        InitializeGrid();
    }
    
    private void OnDisable()
    {
        EditorPrefs.SetInt(GRID_WIDTH, width);
        EditorPrefs.SetInt(GRID_HEIGHT, height);
        
        if (colorDataContainer != null)
        {
            string path = AssetDatabase.GetAssetPath(colorDataContainer);
            EditorPrefs.SetString(COLOR_DATA_CONTAINER, path);
        }
    }
    
    private void InitializeGrid()
    {
        currentColorDataList = new ColorData[width * height];
        
        for (var index = 0; index < currentColorDataList.Length; index++)
        {
            currentColorDataList[index].ColorType = ColorType.Red;
            currentColorDataList[index].Color = colorDataContainer.itemList[0].Color;
        }
    }
    
    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
                
        width = EditorGUILayout.IntField("Width", width);
        height = EditorGUILayout.IntField("Height", height);
        
        if (EditorGUI.EndChangeCheck())
        {
            InitializeGrid();
        }
        
        colorDataContainer = (ColorDataContainer)EditorGUILayout.ObjectField("Color Data Container", colorDataContainer, typeof(ColorDataContainer), false);
                
        EditorGUI.BeginChangeCheck();
        levelData = (LevelData)EditorGUILayout.ObjectField("Level Data", levelData, typeof(LevelData), false);
        if (EditorGUI.EndChangeCheck())
        {
            if (levelData != null)
            {
                currentColorDataList = levelData.Copy();
                Selection.activeObject = levelData;
            }
            else
            {
                for (var index = 0; index < currentColorDataList.Length; index++)
                {
                    currentColorDataList[index].ColorType = ColorType.Red;
                    currentColorDataList[index].Color = colorDataContainer.itemList[0].Color;
                }
            }
        }
        
        if (levelData == null)
        {
            DrawCreateLevelDataButton();
            
            return;
        }
        
        
        EditorGUILayout.Space();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        
        float availableWidth = position.width - 20;
        float availableHeight = position.height - 200;
        
        int cellWidth = Mathf.FloorToInt(availableWidth / width);
        int cellHeight = Mathf.FloorToInt(availableHeight / height);
            
        for (int y = 0; y < height; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                Color baseColor =  EditorHelper.ToGuiColor(currentColorDataList[index].Color);
                GUI.backgroundColor = baseColor;
                    
                if (GUILayout.Button("", GUILayout.Width(cellWidth), GUILayout.Height(cellHeight)))
                {
                    HandleCellClickEvent(index);
                }
                    
                GUI.backgroundColor = Color.white;
            }
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.Space(10);
         
        DrawSaveButton();
        
        EditorGUILayout.EndScrollView();
    }

    private void DrawSaveButton()
    {
        EditorGUILayout.BeginHorizontal();
        
        Color color = new Color(0, 0.5803921568627451f, 0.19607843137254902f,1);
        GUI.backgroundColor =  EditorHelper.ToGuiColor(color);
        
        int buttonWidth = (int)position.width - 10;
        int buttonHeight = 50;
        
        if (GUILayout.Button("Save", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
        {
            SaveLevelData();
        }
        
        EditorGUILayout.EndHorizontal();
    }
    
    private void SaveLevelData()
    {
        if (levelData != null)
        {
            ColorData[] colorDataArray = new ColorData[currentColorDataList.Length];
                
            for (var index = 0; index < currentColorDataList.Length; index++)
            {
                colorDataArray[index].ColorType = currentColorDataList[index].ColorType;
                colorDataArray[index].Color = currentColorDataList[index].Color;
            }
            
            levelData.colorDataList = null;
            levelData.colorDataList = colorDataArray;
                
            EditorUtility.SetDirty(levelData); 
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
                
            Debug.Log("Level data saved!");
        }
        else
        {
            Debug.LogWarning("Level data is null!");
        }
    }
    
    private void DrawCreateLevelDataButton()
    {
        GUILayout.BeginVertical();         
        GUILayout.FlexibleSpace();         

        GUILayout.BeginHorizontal();       
        GUILayout.FlexibleSpace();         

        if (GUILayout.Button("Create Level Data", GUILayout.Width(150), GUILayout.Height(42)))
        {
            levelData = LevelDataCreator.CreateNewLevelAsset(width , height); 
            
            for (var i = 0; i < levelData.colorDataList.Length; i++)
            {
                levelData.colorDataList[i].ColorType = ColorType.Red;
                levelData.colorDataList[i].Color = colorDataContainer.itemList[0].Color;
            }
            
        }

        GUILayout.FlexibleSpace();         
        GUILayout.EndHorizontal();         
    
        GUILayout.FlexibleSpace();         
        GUILayout.EndVertical();
    }
        
    private void HandleCellClickEvent(int index)
    {
        int current = (int)currentColorDataList[index].ColorType;
        
        int first = (int)ColorType.Red;
        int last = (int)ColorType.Purple;
        
        int next = current + 1;
        if (next > last)
            next = first;
        
        currentColorDataList[index].ColorType = (ColorType)next;
        currentColorDataList[index].Color = colorDataContainer.itemList[--next].Color;
                
        //levelData.colorDataList[index] = currentColorDataList[index];
    }
    
    
    
}

public static class EditorHelper
{
    private const float UNITY_GUI_MULTIPLIER = 0.345f;
    
    // IMGUI용 보정 색상 반환
    public static Color ToGuiColor(Color targetColor)
    {
        float correction = 1.0f / UNITY_GUI_MULTIPLIER;
        return new Color(
            targetColor.r * correction,
            targetColor.g * correction,
            targetColor.b * correction,
            targetColor.a
        );
    }
    
}

