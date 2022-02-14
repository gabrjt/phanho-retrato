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
    [ShowNonSerializedField] bool _fadeIn = true;
    RenderTextureContainer _renderTextureContainer;
    SceneLoader _sceneLoader;
    Texture _texture;

    async void OnEnable()
    {
        var sceneLoaderAsyncOperation = _sceneLoaderReference.LoadAssetAsync<SceneLoader>();
        var renderTextureContainerAsyncOperation = _renderTextureContainerReference.LoadAssetAsync<RenderTextureContainer>();

        bool LoadAssetsCompleted()
        {
            return sceneLoaderAsyncOperation.IsDone && renderTextureContainerAsyncOperation.IsDone;
        }

        var cancelled = await UniTask.WaitUntil(LoadAssetsCompleted, PlayerLoopTiming.Initialization, this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();

        if (cancelled)
        {
            return;
        }

        _sceneLoader = sceneLoaderAsyncOperation.Result;
        _renderTextureContainer = renderTextureContainerAsyncOperation.Result;

        _sceneLoader.SceneLoaded += OnSceneLoaded;
        _renderTextureContainer.TextureRendered += OnTextureRendered;
    }

    void OnDisable()
    {
        Assert.IsNotNull(_sceneLoader);
        Assert.IsNotNull(_renderTextureContainer);

        _sceneLoader.SceneLoaded -= OnSceneLoaded;
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

    void OnSceneLoaded()
    {
        _fadeIn = true;
    }

    bool IsStopped()
    {
        return !_uiTransitionEffect.effectPlayer.play;
    }

    void OnTextureRendered(Canvas canvas, RenderTexture renderTexture)
    {
        canvas.gameObject.SetActive(false);

        _texture = _uiTransitionEffect.transitionTexture = _image.texture = renderTexture.ToTexture2D();

        if (_fadeIn)
        {
            _fadeIn = false;

            FadeIn(canvas);
        }
        else
        {
            FadeOut();
        }
    }

    async void FadeIn(Canvas canvas)
    {
        _uiTransitionEffect.Show();

        var cancelled = await UniTask.WaitUntil(IsStopped, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();

        if (!cancelled)
        {
            OnFadeFinished();

            canvas.gameObject.SetActive(true);
        }
    }

    async void FadeOut()
    {
        if (!_sceneLoader.TryUnloadCurrentScene(out _))
        {
            return;
        }

        _image.gameObject.SetActive(true);
        _uiTransitionEffect.Hide();

        var cancelled = await UniTask.WaitUntil(IsStopped, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();

        if (!cancelled)
        {
            OnFadeFinished();

            _sceneLoader.LoadNextScene();
        }
    }

    void OnFadeFinished()
    {
        DestroyImmediate(_texture);

        _texture = _uiTransitionEffect.transitionTexture = _image.texture = null;
    }
}