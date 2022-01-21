using System;
using Coffee.UIEffects;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage), typeof(UITransitionEffect))]
public class SceneTransition : MonoBehaviour
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

        _uiTransitionEffect.Hide();

        var cancelled = await UniTask.WaitUntil(IsStopped).SuppressCancellationThrow();

        _uiTransitionEffect.effectFactor = 0;

        if (!cancelled)
        {
            _sceneLoader.LoadNextScene();
        }
    }
}