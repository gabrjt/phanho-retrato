using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class Mirror : MonoBehaviour
{
    [SerializeField] [Required] CharacterBodyParts _bodyParts;
    [SerializeField] [Required] Image _head;
    [SerializeField] [Required] Image _arms;
    [SerializeField] [Required] Image _legs;
    [SerializeField] [Required] Image _tail;

    void OnEnable()
    {
        _head.enabled = _head.sprite = _bodyParts.Head ? _bodyParts.Head.ResultImage : null;
        _arms.enabled = _arms.sprite = _bodyParts.Arms ? _bodyParts.Arms.ResultImage : null;
        _legs.enabled = _legs.sprite = _bodyParts.Legs ? _bodyParts.Legs.ResultImage : null;
        _tail.enabled = _tail.sprite = _bodyParts.Tail ? _bodyParts.Tail.ResultImage : null;
    }

    void OnDisable()
    {
        _head.sprite = _arms.sprite = _legs.sprite = _tail.sprite = null;
    }
}