using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBodyPartSelector : MonoBehaviour
{
    [SerializeField] [Required] Image _image;
    [SerializeField] SpriteUnityEvent _selected;

    public void Select()
    {
        _selected.Invoke(_image.sprite);
    }
}