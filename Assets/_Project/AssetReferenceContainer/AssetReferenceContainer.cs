using System;
using System.Collections.Generic;
using System.Threading;
using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;

[CreateAssetMenu]
public class AssetReferenceContainer : ScriptableObject
{
    [SerializeField] AssetReference[] _assetReferences;
    readonly CancellationTokenContainer _cancellationToken = new();
    [NonSerialized] [ShowNonSerializedField] internal int Index;

    public int NextIndex => math.clamp((Index + 1) % _assetReferences.Length, 0, _assetReferences.Length - 1);

    public int PreviousIndex => math.clamp((Index - 1) % _assetReferences.Length, 0, _assetReferences.Length - 1);

    public int Length => _assetReferences.Length;

    public AssetReference this[int key] => _assetReferences[key];

    public IReadOnlyList<AssetReference> AssetReferences => _assetReferences;

    public AssetReference Current => _assetReferences[Index];

    public CancellationToken CancellationToken => _cancellationToken.CancellationToken;

    void OnDisable()
    {
        _cancellationToken.Cancel();
    }

    void OnValidate()
    {
        Assert.IsNotNull(_assetReferences);
        Assert.AreNotEqual(_assetReferences.Length, 0);
    }

    public void Cancel()
    {
        _cancellationToken.Cancel();
    }

    public void ResetIndex()
    {
        Index = default;
    }

    public void ResetCancellationToken()
    {
        _cancellationToken.Reset();
    }

    public bool IsValidIndex()
    {
        return IsValidIndex(Index);
    }

    public bool IsValidIndex(int index)
    {
        return _assetReferences.IsValidIndex(index);
    }
}