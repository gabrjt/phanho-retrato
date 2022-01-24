using Coffee.UIEffects;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UITransitionEffect), typeof(Image))]
public class UITransitionEffectLock : MonoBehaviour
{
    [SerializeField] [Required] UITransitionEffect _uiTransitionEffect;
    [SerializeField] [Required] Image _image;
    [SerializeField] bool _locked = true;
    readonly CancellationTokenContainer _cancellationToken = new();

    void OnDisable()
    {
        _cancellationToken.Cancel();
    }

    void OnValidate()
    {
        _uiTransitionEffect = GetComponent<UITransitionEffect>();
        _image = GetComponent<Image>();
    }

    public void SetLocked(bool locked)
    {
        _locked = locked;
    }

    public bool IsUnlocked()
    {
        return !_locked;
    }

    public async void WaitUnlockAndShow()
    {
        _cancellationToken.Cancel();

        var cancelled = await UniTask.WaitUntil(IsUnlocked, PlayerLoopTiming.Update, _cancellationToken.CancellationToken).SuppressCancellationThrow();

        if (cancelled)
        {
            return;
        }

        _image.enabled = true;

        Show();
    }

    public void Show()
    {
        if (_locked)
        {
            return;
        }

        SetLocked(true);

        _uiTransitionEffect.Show();
    }

    public void Hide()
    {
        if (_locked)
        {
            return;
        }

        SetLocked(true);

        _uiTransitionEffect.Hide();
    }
}