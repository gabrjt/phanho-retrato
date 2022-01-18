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
public partial class SceneLoader : ScriptableObject
{
    [SerializeField] AssetReference[] _scenes;
    readonly CancellationTokenContainer _cancellationToken = new();
    int _index = -1;

    int NextIndex => math.clamp((_index + 1) % _scenes.Length, 0, _scenes.Length - 1);

    int PreviousIndex => math.clamp((_index - 1) % _scenes.Length, 0, _scenes.Length - 1);

    void OnDisable()
    {
        _cancellationToken.Cancel();
    }

    [Button]
    public void LoadNextScene()
    {
        LoadScene(NextIndex);
    }

    [Button]
    public void LoadPreviousScene()
    {
        LoadScene(PreviousIndex);
    }

    async void LoadScene(int index)
    {
        if (!Application.isPlaying)
        {
            return;
        }

        _cancellationToken.Cancel();

        Assert.IsTrue(_scenes.IsValidIndex(index));

        if (TryUnloadCurrentScene(out var asyncOperationHandle))
        {
            var (cancelled, _) = await asyncOperationHandle.WithCancellation(_cancellationToken.CancellationToken).SuppressCancellationThrow();

            if (cancelled)
            {
                return;
            }
        }

        {
            _cancellationToken.Reset();

            var (cancelled, sceneInstance) = await _scenes[_index = index].LoadSceneAsync(LoadSceneMode.Additive).WithCancellation(_cancellationToken.CancellationToken).SuppressCancellationThrow();

            if (cancelled)
            {
                return;
            }

            SceneManager.SetActiveScene(sceneInstance.Scene);
        }
    }

    bool TryUnloadCurrentScene(out AsyncOperationHandle<SceneInstance> asyncOperationHandle)
    {
        asyncOperationHandle = default;

        if (!_scenes.IsValidIndex(_index))
        {
            return false;
        }

        var scene = _scenes[_index];

        if (!scene.IsValid())
        {
            return false;
        }

        asyncOperationHandle = scene.UnLoadScene();

        return true;
    }

    public void ReloadScene()
    {
        LoadScene(_index);
    }
}