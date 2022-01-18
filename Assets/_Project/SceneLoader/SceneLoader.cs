using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public partial class SceneLoader : ScriptableObject
{
    [SerializeField] AssetReferenceContainer _scenesContainer;

    void ResetIndex()
    {
        _scenesContainer.ResetIndex();
    }

    [Button]
    public void LoadFirstScene()
    {
        ResetIndex();
        ReloadScene();
    }

    [Button]
    public void LoadNextScene()
    {
        LoadScene(_scenesContainer.NextIndex);
    }

    [Button]
    public void LoadPreviousScene()
    {
        LoadScene(_scenesContainer.PreviousIndex);
    }

    async void LoadScene(int index)
    {
        Assert.IsTrue(Application.isPlaying);

        _scenesContainer.Cancel();

        Assert.IsTrue(_scenesContainer.IsValidIndex(index));

        if (TryUnloadCurrentScene(out var asyncOperationHandle))
        {
            var (cancelled, _) = await asyncOperationHandle.WithCancellation(_scenesContainer.CancellationToken).SuppressCancellationThrow();

            if (cancelled)
            {
                return;
            }
        }

        {
            _scenesContainer.ResetCancellationToken();

            var (cancelled, sceneInstance) = await _scenesContainer[index].LoadSceneAsync(LoadSceneMode.Additive).WithCancellation(_scenesContainer.CancellationToken).SuppressCancellationThrow();

            if (cancelled)
            {
                return;
            }

            SceneManager.SetActiveScene(sceneInstance.Scene);

            _scenesContainer.Index = index;
        }
    }

    bool TryUnloadCurrentScene(out AsyncOperationHandle<SceneInstance> asyncOperationHandle)
    {
        asyncOperationHandle = default;

        if (!_scenesContainer.IsValidIndex())
        {
            return false;
        }

        var scene = _scenesContainer.Current;

        if (!scene.IsValid())
        {
            return false;
        }

        asyncOperationHandle = scene.UnLoadScene();

        return true;
    }

    public void ReloadScene()
    {
        LoadScene(_scenesContainer.Index);
    }
}