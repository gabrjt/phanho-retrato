using System.IO;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[RequireComponent(typeof(Mirror))]
public class MirrorResultGenerator : MonoBehaviour
{
    [SerializeField] [Required] Mirror _mirror;
    [SerializeField] CharacterBodyPart[] _headArray;
    [SerializeField] CharacterBodyPart[] _armsArray;
    [SerializeField] CharacterBodyPart[] _legsArray;
    [SerializeField] CharacterBodyPart[] _tailArray;
    readonly CancellationTokenContainer _cancellationToken = new();
    Random _random = new (0xF);

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

    public void SetRandomCharacterBodyParts()
    {
        var characterBodyParts = _mirror.CharacterBodyParts;

        characterBodyParts.Dispose();

        characterBodyParts.SetHead(_headArray[_random.NextInt(_headArray.Length)]);
        characterBodyParts.SetArms(_armsArray[_random.NextInt(_armsArray.Length)]);
        characterBodyParts.SetLegs(_legsArray[_random.NextInt(_legsArray.Length)]);
        characterBodyParts.SetTail(_tailArray[_random.NextInt(_tailArray.Length)]);
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

                        var cancelled = await UniTask.NextFrame(PlayerLoopTiming.Update, _cancellationToken.CancellationToken).SuppressCancellationThrow();

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