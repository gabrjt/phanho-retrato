using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu]
public class PaginatedText : ScriptableObject
{
    [SerializeField] [ResizableTextArea] string[] _pages;
    [ShowNonSerializedField] int _index;

    public void Reset()
    {
        _index = default;
    }

    public bool TryGetNextPage(out string page)
    {
        page = string.Empty;

        if (_pages.IsValidIndex(_index))
        {
            page = _pages[_index++];

            return true;
        }

        Assert.AreEqual(_index, _pages.Length);

        return false;
    }
}