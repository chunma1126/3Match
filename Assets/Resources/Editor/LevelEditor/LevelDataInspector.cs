using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelData))]
public class LevelDataInspector : Editor
{
    public override void OnInspectorGUI()
    {
        LevelData currentLevelData = (LevelData)target;
        
        Vector2Int size = currentLevelData.boardSize;
        ColorData[] dataList = currentLevelData.colorDataList;
        
        if (dataList == null)
        {
            EditorGUILayout.HelpBox("colorDataList size does not match boardSize", MessageType.Warning);
            return;
        }
        
        int cellSize = 65;
        
        EditorGUILayout.LabelField("Color Datas", EditorStyles.boldLabel);
        
        for (int y = 0; y < size.y; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < size.x; x++)
            {
                int index = y * size.x + x;
                ColorData data = dataList[index];

                GUI.backgroundColor = EditorHelper.ToGuiColor(data.Color);
                
                GUILayout.Button("", GUILayout.Width(cellSize), GUILayout.Height(cellSize));

                GUI.backgroundColor = Color.white;
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Open Level Editor", GUILayout.Width(337), GUILayout.Height(40)))
        {
            var editorWindow = LevelEditorWindow.GetWindow<LevelEditorWindow>();
            editorWindow.SetLevelData(currentLevelData);
            
            editorWindow.Show();
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Open Text Editor", GUILayout.Width(337), GUILayout.Height(40)))
        {
            var editorWindow = LevelEditorWindow.GetWindow<TextLevelEditor>();
            editorWindow.SetLevelData(currentLevelData);
            
            editorWindow.Show();
        }
        
        EditorGUILayout.EndHorizontal();
        
        
    }
}