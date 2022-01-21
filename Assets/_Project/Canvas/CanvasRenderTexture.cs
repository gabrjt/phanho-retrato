using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CanvasRenderTexture : MonoBehaviour
{
    [SerializeField] [Required] Canvas _canvas;
    [SerializeField] [Required] Camera _camera;
    [SerializeField] [Required] [Expandable] RenderTextureContainer _renderTextureContainer;

    void OnValidate()
    {
        _canvas = GetComponent<Canvas>();
    }

    public void RenderTexture()
    {
        var clearFlags = _camera.clearFlags;
        var renderMode = _canvas.renderMode;
        var worldCamera = _canvas.worldCamera;

        _camera.clearFlags = CameraClearFlags.Nothing;
        _camera.targetTexture = _renderTextureContainer.RenderTexture;
        _canvas.renderMode = RenderMode.ScreenSpaceCamera;
        _canvas.worldCamera = _camera;

        _camera.Render();

        _renderTextureContainer.InvokeTextureRendered();

        _camera.clearFlags = clearFlags;
        _canvas.renderMode = renderMode;
        _canvas.worldCamera = worldCamera;
        _camera.targetTexture = null;
    }
}