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

        if (dataList == null || dataList.Length != size.x * size.y)
        {
            EditorGUILayout.HelpBox("colorDataList size does not match boardSize", MessageType.Warning);
            return;
        }

        int cellSize = 65;
        
        EditorGUILayout.LabelField("Color Grid", EditorStyles.boldLabel);
        
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

        
    }
}