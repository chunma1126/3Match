using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

#region Types
[Serializable]
public class AssetReferenceAudioSO : AssetReferenceT<AudioSO>
{
    public AssetReferenceAudioSO(string guid) : base(guid)
    {
        
    }
}

[Serializable]
public class AssetReferenceColorDataContainer : AssetReferenceT<ColorDataContainer>
{
    public AssetReferenceColorDataContainer(string guid) : base(guid)
    {
    }
}


#endregion

public static class AssetManager
{
    private static Dictionary<string, AssetHandle> loadedAssets = new();
    
    public static async Task<T> LoadAsync<T>(AssetReference reference) where T : UnityEngine.Object
    {
        string key = reference.AssetGUID;
        
        if (loadedAssets.TryGetValue(key, out var handle))
        {
            handle.RefCount++;
            return handle.Asset as T;
        }
        
        var op = reference.LoadAssetAsync<T>();
        await op.Task;
        
        if (op.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"[AssetManager] Failed to load asset: {key}");
            return null;
        }
        
        loadedAssets[key] = new AssetHandle(op, op.Result, 1);
        return op.Result;
    }
    
    public static void Release(AssetReference reference)
    {
        string key = reference.AssetGUID;
        if (!loadedAssets.TryGetValue(key, out var handle)) return;

        handle.RefCount--;
        if (handle.RefCount <= 0)
        {
            Addressables.Release(handle.Operation);
            loadedAssets.Remove(key);
        }
    }
    
    public static void ReleaseAll()
    {
        foreach (var kv in loadedAssets.Values)
        {
            Addressables.Release(kv.Operation);
        }
        loadedAssets.Clear();
    }
}

public class AssetHandle
{
    public AsyncOperationHandle Operation { get; }
    public UnityEngine.Object Asset { get; }
    public int RefCount { get; set; }
    
    public AssetHandle(AsyncOperationHandle op, UnityEngine.Object asset, int refCount)
    {
        Operation = op;
        Asset = asset;
        RefCount = refCount;
    }
}
