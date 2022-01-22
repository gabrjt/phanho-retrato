#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public partial class SceneLoader
{
    void OnValidate()
    {
        foreach (var assetReference in _scenesContainer.AssetReferences)
        {
            Assert.AreEqual(assetReference.editorAsset.GetType(), typeof(SceneAsset));
        }
    }

    public bool TrySetIndex()
    {
        _scenesContainer.ResetIndex();

        var activeScene = SceneManager.GetActiveScene();

        for (var index = 0; index < _scenesContainer.Length; index++)
        {
            if (activeScene != SceneManager.GetSceneByPath(AssetDatabase.GUIDToAssetPath(_scenesContainer[index].AssetGUID)))
            {
                continue;
            }

            _scenesContainer.SetIndex(index);

            return true;
        }

        return false;
    }
}
#endif