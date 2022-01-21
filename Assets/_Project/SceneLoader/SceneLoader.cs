using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public partial class SceneLoader : ScriptableObject
{
    [SerializeField] [Required] [Expandable] AssetReferenceContainer _scenesContainer;

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

    async void LoadScene(int index)
    {
        var (success, sceneInstance) = await _scenesContainer.LoadScene(index);

        if (!success)
        {
            return;
        }

        SceneManager.SetActiveScene(sceneInstance.Scene);

#if UNITY_EDITOR
        Bootstrap.SetExpanded(sceneInstance.Scene, true);
#endif
    }

    void ResetIndex()
    {
        _scenesContainer.ResetIndex();
    }
}