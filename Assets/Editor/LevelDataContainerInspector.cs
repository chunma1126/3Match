using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelDataContainer))]
public class LevelDataContainerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if (GUILayout.Button("collect all Level datas",GUILayout.Height(34)))
        {
            LevelData[] levelDatas = (LevelData[])Resources.FindObjectsOfTypeAll(typeof(LevelData));
            if (levelDatas.Length == 0)
            {
                Debug.LogError("ERROR : No level data found");
                return;
            }
            
            (target as LevelDataContainer).levelDataList = levelDatas ;
        }
    }
}