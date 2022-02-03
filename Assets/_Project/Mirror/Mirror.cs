using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class Mirror : MonoBehaviour
{
    [SerializeField] [Required] CharacterBodyParts _characterBodyParts;
    [SerializeField] [Required] Image _head;
    [SerializeField] [Required] Image _arms;
    [SerializeField] [Required] Image _legs;
    [SerializeField] [Required] Image _tail;

    public CharacterBodyParts CharacterBodyParts => _characterBodyParts;

    void OnEnable()
    {
        SetImageSprites();
    }

    void OnDisable()
    {
        _head.sprite = _arms.sprite = _legs.sprite = _tail.sprite = null;
    }

    public void SetImageSprites()
    {
        _head.enabled = _head.sprite = _characterBodyParts.Head ? _characterBodyParts.Head.ResultImage : null;
        _arms.enabled = _arms.sprite = _characterBodyParts.Arms ? _characterBodyParts.Arms.ResultImage : null;
        _legs.enabled = _legs.sprite = _characterBodyParts.Legs ? _characterBodyParts.Legs.ResultImage : null;
        _tail.enabled = _tail.sprite = _characterBodyParts.Tail ? _characterBodyParts.Tail.ResultImage : null;
    }
}