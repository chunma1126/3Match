using UnityEditor;
using UnityEngine;


public static class LevelDataCreator
{
    private const string LEVEL_DATA_ASSET_PATH = "Assets/04.SO/LevelData/";
    private static readonly Color DEFAULT_COLOR = new Color(255/255,82/255,82/255,1);
    
    public static LevelData CreateNewLevelAsset(int width , int height)
    {
        string baseName = "New Level Data";
        string assetPath = "";
        int index = 0;
            
        do
        {
            string numberedName = index == 0 ? baseName : $"{baseName}_{index}";
            assetPath = $"{LEVEL_DATA_ASSET_PATH}/{numberedName}.asset";
            index++;
        }
        while (AssetDatabase.LoadAssetAtPath<LevelData>(assetPath) != null);
        
        LevelData newAsset = ScriptableObject.CreateInstance<LevelData>();
        newAsset.colorDataList = new ColorData[width * height];
    
        for (int i = 0; i < newAsset.colorDataList.Length; i++)
        {
            newAsset.colorDataList[i].ColorType = ColorType.Red;
            newAsset.colorDataList[i].Color = DEFAULT_COLOR;
        }


        newAsset.boardSize = new Vector2Int(width, height);
        newAsset.name = baseName;
        
        AssetDatabase.CreateAsset(newAsset, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Selection.activeObject = newAsset;
        
        return newAsset;
    }
}