using Coffee.UIEffects;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage), typeof(UITransitionEffect))]
public class SceneTransition : MonoBehaviour
{
    [SerializeField] AssetReference _sceneLoaderReference;
    [SerializeField] AssetReference _renderTextureContainerReference;
    [SerializeField] [Required] RawImage _rawImage;
    [SerializeField] [Required] UITransitionEffect _uiTransitionEffect;
    RenderTextureContainer _renderTextureContainer;
    SceneLoader _sceneLoader;

    void OnEnable()
    {
        _sceneLoader = _sceneLoaderReference.LoadAssetAsync<SceneLoader>().WaitForCompletion();
        _renderTextureContainer = _renderTextureContainerReference.LoadAssetAsync<RenderTextureContainer>().WaitForCompletion();
        _rawImage.texture = _renderTextureContainer.RenderTexture;

        _renderTextureContainer.TextureRendered += OnTextureRendered;
    }

    void OnDisable()
    {
        Assert.IsNotNull(_sceneLoader);
        Assert.IsNotNull(_renderTextureContainer);

        _renderTextureContainer.TextureRendered -= OnTextureRendered;

        _sceneLoader = null;
        _rawImage.texture = null;
        _renderTextureContainer = null;

        _renderTextureContainerReference.ReleaseAsset();
        _sceneLoaderReference.ReleaseAsset();
    }

    void OnValidate()
    {
        _rawImage = GetComponent<RawImage>();
        _uiTransitionEffect = GetComponent<UITransitionEffect>();
    }

    bool IsStopped()
    {
        return !_uiTransitionEffect.effectPlayer.play;
    }

    async void OnTextureRendered(RenderTexture renderTexture)
    {
        Assert.AreEqual(renderTexture, _renderTextureContainer.RenderTexture);

        if (!_sceneLoader.TryUnloadCurrentScene(out _))
        {
            return;
        }

        _uiTransitionEffect.effectFactor = 1;
        _uiTransitionEffect.Hide();

        var cancelled = await UniTask.WaitUntil(IsStopped).SuppressCancellationThrow();

        _uiTransitionEffect.effectFactor = 0;

        if (!cancelled)
        {
            _sceneLoader.LoadNextScene();
        }
    }
}