using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(MirrorResultGenerator))]
public class SessionSaveTests : MonoBehaviour
{
    [SerializeField] [Required] MirrorResultGenerator _mirrorResultGenerator;
    [SerializeField] [Required] SessionSave _sessionSave;

    void OnEnable()
    {
        _sessionSave.Clear();

        for (var i = 0; i < 0xFF; i++)
        {
            _sessionSave.Begin();
            _mirrorResultGenerator.SetRandomCharacterBodyParts();
            _sessionSave.End(i.ToString(), $"{i}@{nameof(i)}.com");
        }
    }

    void OnDisable()
    {
        _sessionSave.Cancel();
    }

    void OnValidate()
    {
        _mirrorResultGenerator = GetComponent<MirrorResultGenerator>();
    }
}