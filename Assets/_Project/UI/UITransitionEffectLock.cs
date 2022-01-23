using Coffee.UIEffects;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(UITransitionEffect))]
public class UITransitionEffectLock : MonoBehaviour
{
    [SerializeField] [Required] UITransitionEffect _uiTransitionEffect;
    [ShowNonSerializedField] bool _locked;

    void OnValidate()
    {
        _uiTransitionEffect = GetComponent<UITransitionEffect>();
    }

    public void SetLocked(bool locked)
    {
        _locked = locked;
    }

    public void Show()
    {
        if (_locked)
        {
            return;
        }

        SetLocked(true);

        _uiTransitionEffect.Show();
    }

    public void Hide()
    {
        if (_locked)
        {
            return;
        }

        SetLocked(true);

        _uiTransitionEffect.Hide();
    }
}