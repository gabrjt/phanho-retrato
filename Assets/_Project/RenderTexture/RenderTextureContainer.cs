using System;
using UnityEngine;

[CreateAssetMenu]
public class RenderTextureContainer : ScriptableObject
{
    [SerializeField] int _depth = 24;
    public event Action<Canvas, RenderTexture> TextureRendered;

    public void RenderTexture(Canvas canvas)
    {
        var camera = canvas.worldCamera;
        var renderTexture = camera.targetTexture = new RenderTexture(Screen.width, Screen.height, _depth);

        renderTexture.Create();

        camera.Render();

        TextureRendered?.Invoke(canvas, renderTexture);

        renderTexture.Release();
    }
}