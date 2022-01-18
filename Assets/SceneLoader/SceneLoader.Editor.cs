#if UNITY_EDITOR

using UnityEditor;
using UnityEngine.Assertions;

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
}

#endif