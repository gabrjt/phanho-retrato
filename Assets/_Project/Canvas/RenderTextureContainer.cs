using System;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu]
public class RenderTextureContainer : ScriptableObject
{
    [SerializeField] [Required] RenderTexture _renderTexture;

    public RenderTexture RenderTexture => _renderTexture;

    public event Action<RenderTexture> TextureRendered;

    public void InvokeTextureRendered()
    {
        TextureRendered?.Invoke(_renderTexture);
    }
}