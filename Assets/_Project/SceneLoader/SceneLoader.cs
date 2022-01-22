using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public partial class SceneLoader : ScriptableObject
{
    [SerializeField] [Required] [Expandable] AssetReferenceContainer _scenesContainer;

    public int Index => _scenesContainer.Index;

    public int NextIndex => _scenesContainer.NextIndex;

    public int PreviousIndex => _scenesContainer.PreviousIndex;

    public event Action SceneLoaded;

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

    public void LoadFirstScene()
    {
        ResetIndex();
        ReloadScene();
    }

    public void ReloadScene()
    {
        LoadScene(_scenesContainer.Index);
    }

    public bool TryUnloadCurrentScene(out AsyncOperationHandle<SceneInstance> asyncOperationHandle)
    {
        return _scenesContainer.TryUnloadCurrentScene(out asyncOperationHandle);
    }

    async void LoadScene(int index)
    {
        var (success, sceneInstance) = await _scenesContainer.LoadScene(index);

        if (!success)
        {
            return;
        }

        SceneManager.SetActiveScene(sceneInstance.Scene);

        SceneLoaded?.Invoke();
        
#if UNITY_EDITOR
        Bootstrap.SetExpanded(sceneInstance.Scene, true);
#endif
    }

    void ResetIndex()
    {
        _scenesContainer.ResetIndex();
    }
}