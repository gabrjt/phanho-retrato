using Coffee.UIEffects;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CharacterBodyPartSelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] [Required] CharacterBodyPart _characterBodyPart;
    [SerializeField] [Required] Button _button;
    [SerializeField] [Required] UIEffect _uiEffect;
    [SerializeField] [Required] Image _image;
    [SerializeField] CharacterBodyPartUnityEvent _selected;
    [ShowNonSerializedField] bool _isSelected;

    void OnValidate()
    {
        _button = GetComponent<Button>();
        _image.sprite = _characterBodyPart.StoryImage;

        Assert.IsNotNull(_image.sprite);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_button.enabled)
        {
            return;
        }

        _uiEffect.effectMode = EffectMode.Sepia;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_button.enabled || _isSelected)
        {
            return;
        }

        _uiEffect.effectMode = EffectMode.None;
    }

    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        _uiEffect.effectMode = selected ? EffectMode.Sepia : EffectMode.None;
    }

    public void Select()
    {
        SetSelected(true);

        _selected.Invoke(_characterBodyPart);
    }
}