using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PaginatedTextContainer : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] PaginatedText _paginatedText;
    [SerializeField] GameObject _panel;
    [SerializeField] TMP_Text _text;
    [SerializeField] UnityEvent _paginationFinished;

    void OnEnable()
    {
        _text.text = string.Empty;

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
        var result = _paginatedText.TryGetNextPage(out var page);

        _panel.gameObject.SetActive(result);

        if (result)
        {
            _text.text = page;

            return;
        }

        _paginationFinished.Invoke();
    }
}