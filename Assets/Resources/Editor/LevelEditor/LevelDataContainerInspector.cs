using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CustomEditor(typeof(LevelDataContainer))]
public class LevelDataContainerInspector : UnityEditor.Editor
{
    [SerializeField] private string labelFilter = "LevelData";
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if (GUILayout.Button("collect all Level datas",GUILayout.Height(34)))
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("AddressableAssetSettings not found!");
                return;
            }
            
            var entries = settings.groups
                .SelectMany(g => g.entries)
                .Where(e => e.labels.Contains(labelFilter))
                .ToArray();
                        
            if (entries.Length == 0)
            {
                Debug.LogError("ERROR : No level data found");
                return;
            }
            
            var assetRefs = entries.Select(e => new AssetReference(e.guid)).ToArray();
            (target as LevelDataContainer).levelDataList = assetRefs;
            
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }
        
    }
}