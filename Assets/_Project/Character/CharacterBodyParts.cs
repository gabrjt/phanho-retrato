using System;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu]
public class CharacterBodyParts : ScriptableObject, IDisposable
{
    [SerializeField] CharacterBodyPart _head;
    [SerializeField] CharacterBodyPart _arms;
    [SerializeField] CharacterBodyPart _legs;
    [SerializeField] CharacterBodyPart _tail;

    public CharacterBodyPart Head => _head;

    public CharacterBodyPart Arms => _arms;

    public CharacterBodyPart Legs => _legs;

    public CharacterBodyPart Tail => _tail;

    public CharacterBodyPartsData Data => new(this);

    public int ID => (int)((_tail ? _tail.ID : 0) * math.pow(4, 0) + (_legs ? _legs.ID : 0) * math.pow(4, 1) + (_arms ? _arms.ID : 0) * math.pow(4, 2) + (_head ? _head.ID : 0) * math.pow(4, 3));

    void OnEnable()
    {
        Dispose();
    }

    public void Dispose()
    {
        _head = _arms = _legs = _tail = null;
    }

    public void SetHead(CharacterBodyPart characterBodyPart)
    {
        _head = characterBodyPart;
    }

    public void SetArms(CharacterBodyPart characterBodyPart)
    {
        _arms = characterBodyPart;
    }

    public void SetLegs(CharacterBodyPart characterBodyPart)
    {
        _legs = characterBodyPart;
    }

    public void SetTail(CharacterBodyPart characterBodyPart)
    {
        _tail = characterBodyPart;
    }

    [Serializable]
    public struct CharacterBodyPartsData
    {
        public int TailID;
        public string TailName;
        public int LegsID;
        public string LegsName;
        public int ArmsID;
        public string ArmsName;
        public int HeadID;
        public string HeadName;
        public int CharacterID;

        public CharacterBodyPartsData(CharacterBodyParts characterBodyParts)
        {
            TailID = characterBodyParts.Tail ? characterBodyParts.Tail.ID : -1;
            TailName = characterBodyParts.Tail ? characterBodyParts.Tail.Name : string.Empty;
            LegsID = characterBodyParts.Legs ? characterBodyParts.Legs.ID : -1;
            LegsName = characterBodyParts.Legs ? characterBodyParts.Legs.Name : string.Empty;
            ArmsID = characterBodyParts.Arms ? characterBodyParts.Arms.ID : -1;
            ArmsName = characterBodyParts.Arms ? characterBodyParts.Arms.Name : string.Empty;
            HeadID = characterBodyParts.Head ? characterBodyParts.Head.ID : -1;
            HeadName = characterBodyParts.Head ? characterBodyParts.Head.Name : string.Empty;
            CharacterID = characterBodyParts.ID;
        }
    }
}