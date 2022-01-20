using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

[RequireComponent(typeof(TextPrinter))]
public class TextPaginator : MonoBehaviour
{
    [SerializeField] [Required] TextPrinter _textPrinter;
    [SerializeField] UnityEvent _paginationFinished;

    void OnEnable()
    {
        var text = _textPrinter.Text;

        text.overflowMode = TextOverflowModes.Page;
        text.pageToDisplay = default;

        _textPrinter.PrintStarted.AddListener(SetTextPrinterMaxIndex);
    }

    void OnDisable()
    {
        _textPrinter.PrintStarted.RemoveListener(SetTextPrinterMaxIndex);
    }

    void OnValidate()
    {
        _textPrinter = GetComponent<TextPrinter>();

        Assert.IsNotNull(_textPrinter);
    }

    void SetTextPrinterMaxIndex()
    {
        var text = _textPrinter.Text;
        var textInfo = text.textInfo;

        if (text.pageToDisplay == textInfo.pageCount)
        {
            _paginationFinished.Invoke();

            text.pageToDisplay = default;

            return;
        }

        _textPrinter.MaxIndex = textInfo.pageInfo[text.pageToDisplay++].lastCharacterIndex + 1;
    }
}