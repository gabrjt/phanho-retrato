using NaughtyAttributes;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Mirror))]
public class MirrorText : MonoBehaviour
{
    [SerializeField] [Required] Mirror _mirror;
    [SerializeField] [Required] TMP_Text _text;
    [SerializeField] bool showID;

    void OnEnable()
    {
        var result = showID ? _mirror.CharacterBodyParts.ID + 1 : 1;

        _text.text = $"{result}/256";
    }

    void OnValidate()
    {
        _mirror = GetComponent<Mirror>();
    }
}