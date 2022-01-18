#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public partial class SceneLoader
{
    void OnValidate()
    {
        Assert.IsNotNull(_scenes);
        Assert.AreNotEqual(_scenes.Length, 0);

        foreach (var assetReference in _scenes)
        {
            Assert.AreEqual(assetReference.editorAsset.GetType(), typeof(SceneAsset));
        }
    }

    public void SetIndex()
    {
        _index = default;

        var activeScene = SceneManager.GetActiveScene();

        for (var index = 0; index < _scenes.Length; index++)
        {
            if (activeScene != SceneManager.GetSceneByPath(AssetDatabase.GUIDToAssetPath(_scenes[index].AssetGUID)))
            {
                continue;
            }

            _index = index;

            break;
        }
    }
}
#endif