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
    public enum TextPrinterState
    {
        Idle,
        Printing
    }

    [SerializeField] [Required] TMP_Text _text;
    [SerializeField] [MinValue(0)] int _delayMilliseconds = 10;
    [SerializeField] UnityEvent _printStarted;
    [SerializeField] UnityEvent _printFinished;
    readonly CancellationTokenContainer _cancellationToken = new();
    [NonSerialized] [ShowNonSerializedField] public int FirstIndex;
    [NonSerialized] [ShowNonSerializedField] public int LastIndex;
    [NonSerialized] [ShowNonSerializedField] public bool PrintOnEnable = true;

    public UnityEvent PrintStarted => _printStarted;

    public UnityEvent PrintFinished => _printFinished;

    bool HasDelay => _delayMilliseconds > 0;

    public TMP_Text Text => _text;

    [field: ShowNonSerializedField] public TextPrinterState State { get; private set; } = TextPrinterState.Idle;

    async void OnEnable()
    {
        _text.maxVisibleCharacters = FirstIndex = default;
        State = TextPrinterState.Idle;

        // Must wait for render to get text info properly
        var cancelled = await UniTask.WaitForEndOfFrame(_cancellationToken.CancellationToken).SuppressCancellationThrow();

        if (cancelled)
        {
            return;
        }

        LastIndex = _text.textInfo.characterCount;

        Enabled?.Invoke();

        if (!PrintOnEnable)
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
        _text = GetComponent<TMP_Text>();

        Assert.IsNotNull(_text);
        Assert.IsFalse(string.IsNullOrEmpty(_text.text));
    }

    public event Action Enabled;

    public async void Print()
    {
        if (State == TextPrinterState.Printing)
        {
            _cancellationToken.Cancel();

            return;
        }

        Assert.AreNotEqual(LastIndex, 0);

        if (FirstIndex == LastIndex)
        {
            return;
        }

        State = TextPrinterState.Printing;

        _printStarted.Invoke();

        if (HasDelay)
        {
            while (FirstIndex < LastIndex)
            {
                _text.maxVisibleCharacters = FirstIndex++;

                var cancelled = await UniTask.Delay(_delayMilliseconds, false, PlayerLoopTiming.Update, _cancellationToken.CancellationToken).SuppressCancellationThrow();

                if (cancelled)
                {
                    break;
                }
            }
        }

        _text.maxVisibleCharacters = FirstIndex = LastIndex;

        State = TextPrinterState.Idle;

        _printFinished.Invoke();
    }
}