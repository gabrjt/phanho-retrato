using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;

[CreateAssetMenu]
public class AssetReferenceContainer : ScriptableObject
{
    [SerializeField] AssetReference[] _assetReferences;

    public int Length => _assetReferences.Length;

    public IReadOnlyList<AssetReference> AssetReferences => _assetReferences;

    public AssetReference this[int key] => _assetReferences[key];

    void OnValidate()
    {
        Assert.IsNotNull(_assetReferences);
        Assert.AreNotEqual(_assetReferences.Length, 0);
    }

    public bool IsValidIndex(int index)
    {
        return _assetReferences.IsValidIndex(index);
    }
}