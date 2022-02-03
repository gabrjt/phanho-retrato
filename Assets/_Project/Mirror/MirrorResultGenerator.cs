using System.IO;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(Mirror))]
public class MirrorResultGenerator : MonoBehaviour
{
    [SerializeField] [Required] Mirror _mirror;
    [SerializeField] CharacterBodyPart[] _headArray;
    [SerializeField] CharacterBodyPart[] _armsArray;
    [SerializeField] CharacterBodyPart[] _legsArray;
    [SerializeField] CharacterBodyPart[] _tailArray;
    readonly CancellationTokenContainer _cancellationToken = new();

    void Start()
    {
        GenerateAllResults();
    }

    void OnDisable()
    {
        _cancellationToken.Cancel();
    }

    void OnValidate()
    {
        _mirror = GetComponent<Mirror>();
    }

    [Button]
    async void GenerateAllResults()
    {
        _cancellationToken.Cancel();

        var characterBodyParts = _mirror.CharacterBodyParts;

        foreach (var head in _headArray)
        {
            characterBodyParts.SetHead(head);

            foreach (var arms in _armsArray)
            {
                characterBodyParts.SetArms(arms);

                foreach (var legs in _legsArray)
                {
                    characterBodyParts.SetLegs(legs);

                    foreach (var tail in _tailArray)
                    {
                        characterBodyParts.SetTail(tail);

                        _mirror.SetImageSprites();

                        ScreenCapture.CaptureScreenshot($"{Application.persistentDataPath}{Path.DirectorySeparatorChar}{characterBodyParts.ID}.png");

                        var cancelled = await UniTask.DelayFrame(1, PlayerLoopTiming.Update, _cancellationToken.CancellationToken).SuppressCancellationThrow();

                        if (cancelled)
                        {
                            return;
                        }
                    }
                }
            }
        }
    }
}