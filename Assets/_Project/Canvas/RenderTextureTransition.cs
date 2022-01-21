using Coffee.UIEffects;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage), typeof(UITransitionEffect))]
public class RenderTextureTransition : MonoBehaviour
{
    [SerializeField] [Required] [Expandable] SceneLoader _sceneLoader;
    [SerializeField] [Required] [Expandable] RenderTextureContainer _renderTextureContainer;
    [SerializeField] [Required] RawImage _rawImage;
    [SerializeField] [Required] UITransitionEffect _uiTransitionEffect;

    void OnEnable()
    {
        _renderTextureContainer.AddListener(OnTextureRendered);
    }

    void OnDisable()
    {
        _renderTextureContainer.RemoveListener(OnTextureRendered);
    }

    void OnValidate()
    {
        _rawImage = GetComponent<RawImage>();
        _uiTransitionEffect = GetComponent<UITransitionEffect>();

        Assert.IsNotNull(_sceneLoader);
        Assert.IsNotNull(_renderTextureContainer);
        Assert.IsNotNull(_rawImage);
        Assert.IsNotNull(_uiTransitionEffect);
    }

    async void OnTextureRendered(RenderTexture renderTexture)
    {
        Assert.AreEqual(renderTexture, _renderTextureContainer.RenderTexture);

        _uiTransitionEffect.Hide();

        bool IsStopped()
        {
            return !_uiTransitionEffect.effectPlayer.play;
        }

        var cancelled = await UniTask.WaitUntil(IsStopped).SuppressCancellationThrow();

        if (!cancelled)
        {
            _sceneLoader.LoadNextScene();
        }
    }
}