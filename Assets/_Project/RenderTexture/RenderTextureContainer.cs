using System;
using UnityEngine;

[CreateAssetMenu]
public class RenderTextureContainer : ScriptableObject
{
    [SerializeField] int _depth = 24;
    public event Action<Canvas, RenderTexture> TextureRendered;

    public bool HasListener()
    {
        return TextureRendered != null;
    }

    public void RenderTexture(Canvas canvas)
    {
        var camera = canvas.worldCamera;
        var renderTexture = camera.targetTexture = new RenderTexture(Screen.width, Screen.height, _depth);

        renderTexture.Create();

        canvas.gameObject.SetActive(true);

        camera.Render();
        camera.targetTexture = null;

        TextureRendered?.Invoke(canvas, renderTexture);

        renderTexture.Release();
    }
}