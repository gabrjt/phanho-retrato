using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [SerializeField] [Required] CharacterBodyParts _bodyParts;
    [SerializeField] [Required] Image _self;
    [SerializeField] [Required] Image _head;
    [SerializeField] [Required] Image _arms;
    [SerializeField] [Required] Image _legs;
    [SerializeField] [Required] Image _tail;

    void OnEnable()
    {
        _head.enabled = _head.sprite = _bodyParts.Head ? _bodyParts.Head.StoryImage : null;
        _arms.enabled = _arms.sprite = _bodyParts.Arms ? _bodyParts.Arms.StoryImage : null;
        _legs.enabled = _legs.sprite = _bodyParts.Legs ? _bodyParts.Legs.StoryImage : null;
        _tail.enabled = _tail.sprite = _bodyParts.Tail ? _bodyParts.Tail.StoryImage : null;
        _self.enabled = !(_head.enabled && _arms.enabled && _legs.enabled && _tail.enabled);
    }

    void OnDisable()
    {
        _head.sprite = _arms.sprite = _legs.sprite = _tail.sprite = null;
    }
}