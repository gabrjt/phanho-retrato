using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public partial class SceneLoader : Singleton<SceneLoader>
{
    [SerializeField] AssetReference[] _scenes;
    int _index = -1;

    int NextIndex => math.clamp((_index + 1) % _scenes.Length, 0, _scenes.Length - 1);

    int PreviousIndex => math.clamp((_index - 1) % _scenes.Length, 0, _scenes.Length - 1);

    bool IsValidIndex => _index >= 0 && _index < _scenes.Length;

    void Awake()
    {
        LoadNextScene();
    }

    [Button]
    void LoadNextScene()
    {
        LoadScene(NextIndex);
    }

    [Button]
    void LoadPreviousScene()
    {
        LoadScene(PreviousIndex);
    }

    async void LoadScene(int index)
    {
        if (_index == index)
        {
            return;
        }

        if (TryUnloadCurrentScene(out var asyncOperationHandle))
        {
            var (cancelled, _) = await asyncOperationHandle.WithCancellation(this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();

            if (cancelled)
            {
                return;
            }

            LoadSceneInternal();
        }
        else
        {
            LoadSceneInternal();
        }

        void LoadSceneInternal()
        {
            _scenes[_index = index].LoadSceneAsync(LoadSceneMode.Additive);
        }
    }

    bool TryUnloadCurrentScene(out AsyncOperationHandle<SceneInstance> asyncOperationHandle)
    {
        asyncOperationHandle = default;

        if (!IsValidIndex)
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
}