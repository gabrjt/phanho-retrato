using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Mirror : MonoBehaviour
{
    [SerializeField] [Required] CharacterBodyParts _characterBodyParts;
    [SerializeField] [Required] Image _head;
    [SerializeField] [Required] Image _arms;
    [SerializeField] [Required] Image _legs;
    [SerializeField] [Required] Image _tail;
    [SerializeField] [Required] TMP_Text _text;
    [SerializeField] bool showID;

    void OnEnable()
    {
        _head.enabled = _head.sprite = _characterBodyParts.Head ? _characterBodyParts.Head.ResultImage : null;
        _arms.enabled = _arms.sprite = _characterBodyParts.Arms ? _characterBodyParts.Arms.ResultImage : null;
        _legs.enabled = _legs.sprite = _characterBodyParts.Legs ? _characterBodyParts.Legs.ResultImage : null;
        _tail.enabled = _tail.sprite = _characterBodyParts.Tail ? _characterBodyParts.Tail.ResultImage : null;

        var result = showID ? _characterBodyParts.ID + 1 : 1;
        
        _text.text = $"{result}/256";
    }

    void OnDisable()
    {
        _head.sprite = _arms.sprite = _legs.sprite = _tail.sprite = null;
    }
}