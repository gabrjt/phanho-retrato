using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;

public class SpriteCharacter : MonoBehaviour
{
    [SerializeField] SpriteRendererLoader _head;
    [SerializeField] SpriteRendererLoader _arms;
    [SerializeField] SpriteRendererLoader _legs;
    [SerializeField] SpriteRendererLoader _tail;

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
        _head.LoadNextSprite();
        _arms.LoadNextSprite();
        _legs.LoadNextSprite();
        _tail.LoadNextSprite();
    }

    [Button]
    public void LoadPreviousCharacterParts()
    {
        _head.LoadPreviousSprite();
        _arms.LoadPreviousSprite();
        _legs.LoadPreviousSprite();
        _tail.LoadPreviousSprite();
    }
}