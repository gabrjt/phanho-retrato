using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class SaveForm : MonoBehaviour
{
    [SerializeField] [Required] TMP_InputField _username;
    [SerializeField] [Required] TMP_InputField _contact;
    [SerializeField] [Required] SessionSave _sessionSave;

    void OnEnable()
    {
        _username.Select();
    }

    public void Save()
    {
        _sessionSave.End(_username.text, _contact.text);
    }
}