using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;

public class ImageCharacter : MonoBehaviour
{
    [SerializeField] ImageLoader _head;
    [SerializeField] ImageLoader _arms;
    [SerializeField] ImageLoader _legs;
    [SerializeField] ImageLoader _tail;

    void OnValidate()
    {
        Assert.IsTrue(_head.IsValid());
        Assert.IsTrue(_arms.IsValid());
        Assert.IsTrue(_legs.IsValid());
        Assert.IsTrue(_tail.IsValid());
    }

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