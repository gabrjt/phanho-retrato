using NaughtyAttributes;
using UnityEngine;

public class ImageCharacter : MonoBehaviour
{
    [SerializeField] ImageLoader _head;
    [SerializeField] ImageLoader _arms;
    [SerializeField] ImageLoader _legs;
    [SerializeField] ImageLoader _tail;

    [Button]
    public void LoadNextCharacterParts()
    {
        _head.LoadNextImage();
        _arms.LoadNextImage();
        _legs.LoadNextImage();
        _tail.LoadNextImage();
    }

    [Button]
    public void LoadPreviousCharacterParts()
    {
        _head.LoadPreviousImage();
        _arms.LoadPreviousImage();
        _legs.LoadPreviousImage();
        _tail.LoadPreviousImage();
    }
}