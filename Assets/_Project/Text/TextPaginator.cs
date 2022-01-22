using NaughtyAttributes;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TextPrinter))]
public class TextPaginator : MonoBehaviour
{
    [SerializeField] [Required] TextPrinter _textPrinter;
    [SerializeField] UnityEvent _paginationFinished;

    void OnEnable()
    {
        _textPrinter.PrintOnEnable = false;
        _textPrinter.Enabled += PrintNextPage;

        var text = _textPrinter.Text;

        text.overflowMode = TextOverflowModes.Page;
        text.pageToDisplay = default;
    }

    void OnDisable()
    {
        _textPrinter.PrintOnEnable = true;
        _textPrinter.Enabled -= PrintNextPage;

        var text = _textPrinter.Text;

        text.overflowMode = TextOverflowModes.Overflow;
        text.pageToDisplay = default;
    }

    void OnValidate()
    {
        _textPrinter = GetComponent<TextPrinter>();
    }

    [Button]
    public void PrintNextPage()
    {
        if (_textPrinter.State == TextPrinter.TextPrinterState.Printing)
        {
            _textPrinter.Print();

            return;
        }

        var text = _textPrinter.Text;
        var textInfo = text.textInfo;

        if (text.pageToDisplay == textInfo.pageCount)
        {
            _paginationFinished.Invoke();

            return;
        }

        SetIndexes(textInfo.pageInfo[text.pageToDisplay]);

        text.pageToDisplay = math.min(text.pageToDisplay + 1, textInfo.pageCount);

        _textPrinter.Print();
    }

    [Button]
    public void PrintPreviousPage()
    {
        if (_textPrinter.State == TextPrinter.TextPrinterState.Printing)
        {
            _textPrinter.Print();

            return;
        }

        var text = _textPrinter.Text;

        text.pageToDisplay = math.max(text.pageToDisplay - 1, 1);

        SetIndexes(text.textInfo.pageInfo[text.pageToDisplay - 1]);

        _textPrinter.Print();
    }

    void SetIndexes(TMP_PageInfo pageInfo)
    {
        _textPrinter.FirstIndex = pageInfo.firstCharacterIndex;
        _textPrinter.LastIndex = pageInfo.lastCharacterIndex + 1;
    }
}