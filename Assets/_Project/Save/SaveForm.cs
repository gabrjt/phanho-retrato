using System.Net.Mail;
using Coffee.UIEffects;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveForm : MonoBehaviour
{
    [SerializeField] [Required] TMP_InputField _username;
    [SerializeField] [Required] TMP_InputField _contact;
    [SerializeField] [Required] SessionSave _sessionSave;
    [SerializeField] [Required] Button _button;
    [SerializeField] [Required] UITransitionEffect _uiTransitionEffect;

    void OnEnable()
    {
        _username.Select();
    }

    public void Save()
    {
        _sessionSave.End(_username.text, _contact.text);
    }

    public void ValidateContact()
    {
        if (string.IsNullOrEmpty(_contact.text))
        {
            SetValid();

            return;
        }

        if (!IsValidMailAddress(_contact.text) || string.IsNullOrEmpty(_username.text))
        {
            SetInvalid();
        }
        else
        {
            SetValid();
        }
    }

    void SetInvalid()
    {
        _button.interactable = false;

        if (_uiTransitionEffect.effectFactor > 0)
        {
            _uiTransitionEffect.Hide(false);
        }
    }

    void SetValid()
    {
        _button.interactable = true;

        if (_uiTransitionEffect.effectFactor < 1)
        {
            _uiTransitionEffect.Show(false);
        }
    }

    bool IsValidMailAddress(string address)
    {
        var trimmedAddress = address.Trim();

        if (trimmedAddress.EndsWith("."))
        {
            return false;
        }

        try
        {
            return new MailAddress(address).Address == trimmedAddress;
        }
        catch
        {
            return false;
        }
    }
}