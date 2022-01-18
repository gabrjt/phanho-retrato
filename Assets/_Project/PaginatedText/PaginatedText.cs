using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu]
public class PaginatedText : ScriptableObject
{
    [SerializeField] string[] _pages;
    int _index = -1;

    public bool TryGetNextPage(out string page)
    {
        page = string.Empty;

        if (_pages.IsValidIndex(++_index))
        {
            page = _pages[_index];

            return true;
        }

        Assert.AreEqual(_index, _pages.Length);

        _index = -1;

        return false;
    }
}