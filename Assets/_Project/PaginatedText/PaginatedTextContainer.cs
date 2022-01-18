using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PaginatedTextContainer : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] [Expandable] PaginatedText _paginatedText;
    [SerializeField] GameObject _panel;
    [SerializeField] TMP_Text _text;
    [SerializeField] UnityEvent _paginationFinished;

    void OnEnable()
    {
        _text.text = string.Empty;

        _paginatedText.Reset();

        Paginate();
    }

    void OnValidate()
    {
        Assert.IsNotNull(_paginatedText);
        Assert.IsNotNull(_panel);
        Assert.IsNotNull(_text);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Paginate();
    }

    void Paginate()
    {
        if (_paginatedText.TryGetNextPage(out var page))
        {
            _text.text = page;

            return;
        }

        _paginationFinished.Invoke();
    }
}