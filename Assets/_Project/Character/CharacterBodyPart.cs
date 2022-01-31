using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu]
public class CharacterBodyPart : ScriptableObject
{
    [SerializeField] [MinValue(0)] [MaxValue(3)] int _id;
    [SerializeField] string _name;
    [SerializeField] Sprite _storyImage;
    [SerializeField] Sprite _resultImage;

    public int ID => _id;

    public string Name => _name;

    public Sprite StoryImage => _storyImage;

    public Sprite ResultImage => _resultImage;
}