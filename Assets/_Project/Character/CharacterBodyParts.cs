using System;
using UnityEngine;

[CreateAssetMenu]
public class CharacterBodyParts : ScriptableObject, IDisposable
{
    [SerializeField] CharacterBodyPart _head;
    [SerializeField] CharacterBodyPart _arms;
    [SerializeField] CharacterBodyPart _legs;
    [SerializeField] CharacterBodyPart _tail;

    public CharacterBodyPart Head => _head;

    public CharacterBodyPart Arms => _arms;

    public CharacterBodyPart Legs => _legs;

    public CharacterBodyPart Tail => _tail;

    public int ID => _tail.ID * (4 ^ 0) + _legs.ID * (4 ^ 1) + _arms.ID * (4 ^ 2) + _head.ID * (4 ^ 3);

    void OnEnable()
    {
        Dispose();
    }

    public void Dispose()
    {
        _head = _arms = _legs = _tail = null;
    }

    public void SetHead(CharacterBodyPart characterBodyPart)
    {
        _head = characterBodyPart;
    }

    public void SetArms(CharacterBodyPart characterBodyPart)
    {
        _arms = characterBodyPart;
    }

    public void SetLegs(CharacterBodyPart characterBodyPart)
    {
        _legs = characterBodyPart;
    }

    public void SetTail(CharacterBodyPart characterBodyPart)
    {
        _tail = characterBodyPart;
    }
}