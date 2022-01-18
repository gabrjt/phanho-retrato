#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public partial class SceneLoader
{
    void OnValidate()
    {
        Assert.IsNotNull(_scenesContainer);
        Assert.AreNotEqual(_scenesContainer.Length, 0);

        foreach (var assetReference in _scenesContainer.AssetReferences)
        {
            Assert.AreEqual(assetReference.editorAsset.GetType(), typeof(SceneAsset));
        }
    }

    public bool TrySetIndex()
    {
        _scenesContainer.Index = default;

        var activeScene = SceneManager.GetActiveScene();

        for (var index = 0; index < _scenesContainer.Length; index++)
        {
            if (activeScene != SceneManager.GetSceneByPath(AssetDatabase.GUIDToAssetPath(_scenesContainer[index].AssetGUID)))
            {
                continue;
            }

            _scenesContainer.Index = index;

            return true;
        }

        return false;
    }
}
#endif