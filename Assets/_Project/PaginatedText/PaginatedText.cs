using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class PaginatedText : MonoBehaviour
{
    [SerializeField] [Required] TMP_Text _text;
    [SerializeField] UnityEvent _paginationFinished;
    [SerializeField] [MinValue(0)] int _delayMilliseconds;
    readonly CancellationTokenContainer _cancellationToken = new();
    [ShowNonSerializedField] int _index;
    [ShowNonSerializedField] Status _status = Status.Idle;

    bool HasDelay => _delayMilliseconds > 0;

    async void OnEnable()
    {
        _text.pageToDisplay = _text.maxVisibleCharacters = _index = default;
        _status = Status.Idle;

        // Must wait for render to get text info properly
        var cancelled = await UniTask.WaitForEndOfFrame(_cancellationToken.CancellationToken).SuppressCancellationThrow();

        if (cancelled)
        {
            return;
        }

        Print();
    }

    void OnDisable()
    {
        _cancellationToken.Cancel();
    }

    void OnValidate()
    {
        Assert.IsNotNull(_text);
        Assert.IsFalse(string.IsNullOrEmpty(_text.text));
    }

    public async void Print()
    {
        if (_status == Status.Printing)
        {
            _cancellationToken.Cancel();

            return;
        }

        if (_text.pageToDisplay > 0 && _text.pageToDisplay == _text.textInfo.pageCount)
        {
            _paginationFinished.Invoke();

            return;
        }

        _status = Status.Printing;

        var pageInfo = _text.textInfo.pageInfo[_text.pageToDisplay++];
        var lastCharacterIndex = pageInfo.lastCharacterIndex + 1;

        if (HasDelay)
        {
            while (_index <= lastCharacterIndex)
            {
                _text.maxVisibleCharacters = _index++;

                var cancelled = await UniTask.Delay(_delayMilliseconds, false, PlayerLoopTiming.Update, _cancellationToken.CancellationToken).SuppressCancellationThrow();

                if (cancelled)
                {
                    break;
                }
            }
        }

        _text.maxVisibleCharacters = _index = lastCharacterIndex;

        _status = Status.Idle;
    }

    enum Status
    {
        Idle,
        Printing
    }
}