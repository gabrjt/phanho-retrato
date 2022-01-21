using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Canvas))]
public class CanvasRenderTexture : MonoBehaviour
{
    [SerializeField] [Required] Canvas _canvas;
    [SerializeField] [Required] Camera _camera;
    [SerializeField] [Required] [Expandable] RenderTextureContainer _renderTextureContainer;

    void OnValidate()
    {
        _canvas = GetComponent<Canvas>();

        Assert.IsNotNull(_canvas);
        Assert.IsNotNull(_camera);
        Assert.IsNotNull(_renderTextureContainer);
    }

    public void TakeSnapshot()
    {
        _camera.clearFlags = CameraClearFlags.Nothing;
        _camera.targetTexture = _renderTextureContainer.RenderTexture;
        _canvas.renderMode = RenderMode.ScreenSpaceCamera;
        _canvas.worldCamera = _camera;

        _camera.Render();
        
        _camera.gameObject.SetActive(false);
        
        _renderTextureContainer.TextureRendered();

        _camera.targetTexture = null;
    }
}