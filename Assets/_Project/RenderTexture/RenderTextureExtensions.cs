using UnityEngine;

public static class RenderTextureExtensions
{
    public static Texture2D ToTexture2D(this RenderTexture renderTexture)
    {
        var texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);

        var activeRenderTexture = RenderTexture.active;

        RenderTexture.active = renderTexture;

        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        RenderTexture.active = activeRenderTexture;

        return texture2D;
    }
}