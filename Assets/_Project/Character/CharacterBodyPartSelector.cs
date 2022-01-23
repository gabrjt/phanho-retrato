using Coffee.UIEffects;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterBodyPartSelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] [Required] UIEffect _uiEffect;
    [SerializeField] [Required] Image _image;
    [SerializeField] SpriteUnityEvent _selected;
    [ShowNonSerializedField] bool _isSelected;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _uiEffect.effectMode = EffectMode.Sepia;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isSelected)
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

        _selected.Invoke(_image.sprite);
    }
}