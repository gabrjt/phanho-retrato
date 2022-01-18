using System;
using UnityEngine;

[Serializable]
public struct CharacterPart
{
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] AssetReferenceContainer _partsContainer;
}