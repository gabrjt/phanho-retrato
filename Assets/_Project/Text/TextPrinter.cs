using System;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

[RequireComponent(typeof(TMP_Text))]
public class TextPrinter : MonoBehaviour
{
    [SerializeField] [Required] TMP_Text _text;
    [SerializeField] [MinValue(0)] int _delayMilliseconds = 10;
    [SerializeField] UnityEvent _printStarted;
    [SerializeField] UnityEvent _printFinished;
    public bool PrintOnEnable = true;
    readonly CancellationTokenContainer _cancellationToken = new();
    [ShowNonSerializedField] int _index;
    [ShowNonSerializedField] Status _status = Status.Idle;
    [NonSerialized] [ShowNonSerializedField] public int MaxIndex;

    public UnityEvent PrintStarted => _printStarted;

    public UnityEvent PrintFinished => _printFinished;

    bool HasDelay => _delayMilliseconds > 0;

    public TMP_Text Text => _text;

    async void OnEnable()
    {
        _text.maxVisibleCharacters = _index = default;
        _status = Status.Idle;
        MaxIndex = _text.textInfo.characterCount;

        // Must wait for render to get text info properly
        var cancelled = await UniTask.WaitForEndOfFrame(_cancellationToken.CancellationToken).SuppressCancellationThrow();

        if (cancelled)
        {
            return;
        }

        if (PrintOnEnable)
        {
            Print();
        }
    }

    void OnDisable()
    {
        _cancellationToken.Cancel();
    }

    void OnValidate()
    {
        _text = GetComponent<TMP_Text>();

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

        _status = Status.Printing;

        _printStarted.Invoke();

        if (HasDelay)
        {
            while (_index <= MaxIndex)
            {
                _text.maxVisibleCharacters = _index++;

                var cancelled = await UniTask.Delay(_delayMilliseconds, false, PlayerLoopTiming.Update, _cancellationToken.CancellationToken).SuppressCancellationThrow();

                if (cancelled)
                {
                    break;
                }
            }
        }

        _text.maxVisibleCharacters = _index = MaxIndex;

        _status = Status.Idle;

        _printFinished.Invoke();
    }

    enum Status
    {
        Idle,
        Printing
    }
}