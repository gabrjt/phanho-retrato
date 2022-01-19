using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class AssetReferenceContainer : ScriptableObject
{
    [SerializeField] AssetReference[] _assetReferences;
    readonly CancellationTokenContainer _cancellationToken = new();
    [ShowNonSerializedField] internal int Index;

    public int NextIndex => math.clamp((Index + 1) % _assetReferences.Length, 0, _assetReferences.Length - 1);

    public int PreviousIndex => math.clamp((Index - 1) % _assetReferences.Length, 0, _assetReferences.Length - 1);

    public int Length => _assetReferences.Length;

    public AssetReference this[int key] => _assetReferences[key];

    public IEnumerable<AssetReference> AssetReferences => _assetReferences;

    public bool IsCurrentAssetReferenceValid => Current.IsValid();

    AssetReference Current => _assetReferences[Index];

    CancellationToken CancellationToken => _cancellationToken.CancellationToken;

    void OnDisable()
    {
        _cancellationToken.Cancel();
    }

    void OnValidate()
    {
        Assert.IsNotNull(_assetReferences);
        Assert.AreNotEqual(_assetReferences.Length, 0);
    }

    public void ResetIndex()
    {
        Index = default;
    }

    public void ResetCancellationToken()
    {
        _cancellationToken.Reset();
    }

    public async Task<(bool, T)> LoadAsset<T>(int index)
        where T : Object
    {
        Assert.IsTrue(Application.isPlaying);
        Assert.IsTrue(IsValidIndex(index));

        Cancel();

        var assetReference = _assetReferences[index];

        if (assetReference.IsValid())
        {
            return (true, (T)assetReference.Asset);
        }

        var (cancelled, asset) = await assetReference.LoadAssetAsync<T>().WithCancellation(CancellationToken).SuppressCancellationThrow();

        return (!cancelled, asset);
    }

    public bool TryUnloadCurrentAsset()
    {
        if (!IsValidIndex())
        {
            return false;
        }

        var assetReference = Current;

        if (!assetReference.IsValid())
        {
            return false;
        }

        assetReference.ReleaseAsset();

        return true;
    }

    public async Task<(bool, SceneInstance)> LoadScene(int index)
    {
        Assert.IsTrue(Application.isPlaying);
        Assert.IsTrue(IsValidIndex(index));

        Cancel();

        if (TryUnloadCurrentScene(out var asyncOperationHandle))
        {
            var (cancelled, _) = await asyncOperationHandle.WithCancellation(CancellationToken).SuppressCancellationThrow();

            if (cancelled)
            {
                return (false, default);
            }
        }

        {
            ResetCancellationToken();

            var (cancelled, sceneInstance) = await _assetReferences[index].LoadSceneAsync(LoadSceneMode.Additive).WithCancellation(CancellationToken).SuppressCancellationThrow();

            return (!cancelled, sceneInstance);
        }
    }

    bool TryUnloadCurrentScene(out AsyncOperationHandle<SceneInstance> asyncOperationHandle)
    {
        asyncOperationHandle = default;

        if (!IsValidIndex())
        {
            return false;
        }

        var assetReference = Current;

        if (!assetReference.IsValid())
        {
            return false;
        }

        asyncOperationHandle = assetReference.UnLoadScene();

        return true;
    }

    void Cancel()
    {
        _cancellationToken.Cancel();
    }

    bool IsValidIndex()
    {
        return IsValidIndex(Index);
    }

    bool IsValidIndex(int index)
    {
        return _assetReferences.IsValidIndex(index);
    }
}