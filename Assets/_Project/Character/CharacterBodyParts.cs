using System;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu]
public class CharacterBodyParts : ScriptableObject, IDisposable
{
    [SerializeField] Sprite _head;
    [SerializeField] Sprite _arms;
    [SerializeField] Sprite _legs;
    [SerializeField] Sprite _tail;

    public Sprite Head => _head;

    public Sprite Arms => _arms;

    public Sprite Legs => _legs;

    public Sprite Tail => _tail;

    void OnEnable()
    {
        Dispose();
    }

    public void Dispose()
    {
        _head = _arms = _legs = _tail = null;
    }

    public void SetHead(Sprite sprite)
    {
        Assert.IsNull(_head);

        _head = sprite;
    }

    public void SetArms(Sprite sprite)
    {
        Assert.IsNull(_arms);

        _arms = sprite;
    }

    public void SetLegs(Sprite sprite)
    {
        Assert.IsNull(_legs);

        _legs = sprite;
    }

    public void SetTail(Sprite sprite)
    {
        Assert.IsNull(_tail);

        _tail = sprite;
    }
}