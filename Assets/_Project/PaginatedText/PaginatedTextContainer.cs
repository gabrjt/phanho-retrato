using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class PaginatedTextContainer : MonoBehaviour
{
    [SerializeField] [Required] [Expandable] PaginatedText _paginatedText;
    [SerializeField] [Required] TMP_Text _text;
    [SerializeField] UnityEvent _paginationFinished;
    [SerializeField] [MinValue(0)] int _delayMilliseconds;
    readonly CancellationTokenContainer _cancellationToken = new();
    [ShowNonSerializedField] Status _status = Status.Idle;

    bool HasDelay => _delayMilliseconds > 0;

    void OnEnable()
    {
        _text.text = string.Empty;

        _paginatedText.Reset();

        Paginate();
    }

    void OnValidate()
    {
        Assert.IsNotNull(_paginatedText);
        Assert.IsNotNull(_text);
    }

    public async void Paginate()
    {
        _cancellationToken.Cancel();

        if (_status == Status.Paginating)
        {
            return;
        }

        if (_paginatedText.TryGetNextPage(out var page))
        {
            _status = Status.Paginating;

            _text.text = page;

            if (HasDelay)
            {
                for (var index = 0; index < page.Length; index++)
                {
                    _text.maxVisibleCharacters = index + 1;

                    var cancelled = await UniTask.Delay(_delayMilliseconds, DelayType.UnscaledDeltaTime, PlayerLoopTiming.Update, _cancellationToken.CancellationToken).SuppressCancellationThrow();

                    if (cancelled)
                    {
                        break;
                    }
                }
            }

            _text.maxVisibleCharacters = page.Length;

            _status = Status.Idle;

            return;
        }

        _paginationFinished.Invoke();
    }

    enum Status
    {
        Idle,
        Paginating
    }
}