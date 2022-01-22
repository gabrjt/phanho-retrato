using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CanvasRenderTexture : MonoBehaviour
{
    [SerializeField] [Required] Canvas _canvas;
    [SerializeField] [Required] Camera _camera;
    [SerializeField] [Required] RenderTextureContainer _renderTextureContainer;

    async void Start()
    {
        _canvas.gameObject.SetActive(false);

        var cancelled = await UniTask.WaitUntil(_renderTextureContainer.HasListener, PlayerLoopTiming.Initialization, this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();

        if (cancelled)
        {
            return;
        }

        RenderTexture();
    }

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
        _canvas.renderMode = RenderMode.ScreenSpaceCamera;
        _canvas.worldCamera = _camera;

        _renderTextureContainer.RenderTexture(_canvas);

        _camera.clearFlags = clearFlags;
        _canvas.renderMode = renderMode;
        _canvas.worldCamera = worldCamera;
    }
}