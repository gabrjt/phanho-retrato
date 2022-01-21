using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

[CreateAssetMenu]
public class RenderTextureContainer : ScriptableObject
{
    [SerializeField] [Required] RenderTexture _renderTexture;
    [SerializeField] RenderTextureEvent _textureRendered;

    public RenderTexture RenderTexture => _renderTexture;

    void OnValidate()
    {
        Assert.IsNotNull(_renderTexture);
    }

    public void TextureRendered()
    {
        _textureRendered.Invoke(_renderTexture);
    }

    public void AddListener(UnityAction<RenderTexture> action)
    {
        _textureRendered.AddListener(action);
    }

    public void RemoveListener(UnityAction<RenderTexture> action)
    {
        _textureRendered.RemoveListener(action);
    }
}