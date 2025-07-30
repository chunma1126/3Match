using UnityEditor;
using UnityEngine;

public class GridEditorWindow : EditorWindow
{
    private int width = 5;
    private int height = 5;
    private Vector2 scrollPos;

    private ColorDataContainer colorDataContainer;
    
    private int[] colorIndexes; 

    [MenuItem("Tools/Grid Editor")]
    public static void ShowWindow()
    {
        GetWindow<GridEditorWindow>("Grid Editor");
    }
    
    private void OnEnable()
    {
        width = EditorPrefs.GetInt("GridEditorWidth", 5);
        height = EditorPrefs.GetInt("GridEditorHeight", 5);
        string palettePath = EditorPrefs.GetString("GridEditorPalettePath", "");
        if (!string.IsNullOrEmpty(palettePath))
            colorDataContainer = AssetDatabase.LoadAssetAtPath<ColorDataContainer>(palettePath);
        
        InitializeGrid();
    }
    
    private void OnDisable()
    {
        EditorPrefs.SetInt("GridEditorWidth", width);
        EditorPrefs.SetInt("GridEditorHeight", height);

        if (colorDataContainer != null)
        {
            string path = AssetDatabase.GetAssetPath(colorDataContainer);
            EditorPrefs.SetString("GridEditorPalettePath", path);
        }
    }
    
    private void InitializeGrid()
    {
        colorIndexes = new int[width * height];
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("그리드 설정", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        width = EditorGUILayout.IntField("Width", width);
        height = EditorGUILayout.IntField("Height", height);
        
                
        if (EditorGUI.EndChangeCheck())
        {
            InitializeGrid();
        }
        
        colorDataContainer = (ColorDataContainer)EditorGUILayout.ObjectField("Color Palette", colorDataContainer, typeof(ColorDataContainer), false);

        EditorGUILayout.Space();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        if (colorDataContainer != null)
        {
            for (int y = 0; y < height; y++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    int colorIdx = colorIndexes[index] % colorDataContainer.itemList.Length;
                    Color baseColor = colorDataContainer.itemList[colorIdx].Color;
                    GUI.backgroundColor = baseColor;
                    
                    if (GUILayout.Button("", GUILayout.Width(50), GUILayout.Height(50)))
                    {
                        colorIndexes[index]++;
                    }
                    
                    GUI.backgroundColor = Color.white;
                }
                EditorGUILayout.EndHorizontal();
            }

        }
        
        
        EditorGUILayout.EndScrollView();
    }
}
