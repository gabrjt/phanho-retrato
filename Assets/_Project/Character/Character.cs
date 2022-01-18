using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;

public class Character : MonoBehaviour
{
    [SerializeField] CharacterPart _head;
    [SerializeField] CharacterPart _torso;
    [SerializeField] CharacterPart _legs;
    [SerializeField] CharacterPart _tail;

    void OnValidate()
    {
        Assert.IsTrue(_head.IsValid());
        Assert.IsTrue(_torso.IsValid());
        Assert.IsTrue(_legs.IsValid());
        Assert.IsTrue(_tail.IsValid());
    }

    [Button]
    public void LoadNextCharacterParts()
    {
        _head.LoadNextCharacterPart();
        _torso.LoadNextCharacterPart();
        _legs.LoadNextCharacterPart();
        _tail.LoadNextCharacterPart();
    }

    [Button]
    public void LoadPreviousCharacterParts()
    {
        _head.LoadPreviousCharacterPart();
        _torso.LoadPreviousCharacterPart();
        _legs.LoadPreviousCharacterPart();
        _tail.LoadPreviousCharacterPart();
    }
}