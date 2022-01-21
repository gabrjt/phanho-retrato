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
    [SerializeField] [Required] RawImage _image;
    [SerializeField] [Required] UITransitionEffect _uiTransitionEffect;
    [ShowNonSerializedField] int _index;
    RenderTextureContainer _renderTextureContainer;
    SceneLoader _sceneLoader;

    void OnEnable()
    {
        _sceneLoader = _sceneLoaderReference.LoadAssetAsync<SceneLoader>().WaitForCompletion();
        _renderTextureContainer = _renderTextureContainerReference.LoadAssetAsync<RenderTextureContainer>().WaitForCompletion();

        _renderTextureContainer.TextureRendered += OnTextureRendered;
    }

    void OnDisable()
    {
        Assert.IsNotNull(_sceneLoader);
        Assert.IsNotNull(_renderTextureContainer);

        _renderTextureContainer.TextureRendered -= OnTextureRendered;

        _sceneLoader = null;
        _image.texture = null;
        _renderTextureContainer = null;

        _renderTextureContainerReference.ReleaseAsset();
        _sceneLoaderReference.ReleaseAsset();
    }

    void OnValidate()
    {
        _image = GetComponent<RawImage>();
        _uiTransitionEffect = GetComponent<UITransitionEffect>();
    }

    bool IsStopped()
    {
        return !_uiTransitionEffect.effectPlayer.play;
    }

    void OnTextureRendered(Canvas canvas, RenderTexture renderTexture)
    {
        _image.texture = renderTexture.ToTexture2D();

        canvas.gameObject.SetActive(false);

        if (_index == _sceneLoader.Index)
        {
            FadeIn(canvas);

            _index = _sceneLoader.NextIndex;
        }
        else
        {
            FadeOut();
        }
    }

    async void FadeIn(Canvas canvas)
    {
        _uiTransitionEffect.Show();

        var cancelled = await UniTask.WaitUntil(IsStopped).SuppressCancellationThrow();

        if (!cancelled)
        {
            canvas.gameObject.SetActive(true);
        }
    }

    async void FadeOut()
    {
        if (!_sceneLoader.TryUnloadCurrentScene(out _))
        {
            return;
        }

        _uiTransitionEffect.Hide();

        var cancelled = await UniTask.WaitUntil(IsStopped).SuppressCancellationThrow();

        if (!cancelled)
        {
            _sceneLoader.LoadNextScene();
        }
    }
}