﻿using System;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public struct CharacterPart
{
    [SerializeField] [Required] SpriteRenderer _spriteRenderer;
    [SerializeField] [Required] [Expandable] AssetReferenceContainer _characterPartsContainer;

    public void LoadNextCharacterPart()
    {
        LoadAsset(_characterPartsContainer.NextIndex);
    }

    public void LoadPreviousCharacterPart()
    {
        LoadAsset(_characterPartsContainer.PreviousIndex);
    }

    async void LoadAsset(int index)
    {
        if (_characterPartsContainer.IsAssetReferenceValid(index))
        {
            return;
        }

        var (success, sprite) = await _characterPartsContainer.LoadAsset<Sprite>(index);

        if (!success)
        {
            return;
        }

        if (index != _characterPartsContainer.Index)
        {
            _spriteRenderer.sprite = null;

            _characterPartsContainer.TryUnloadCurrentAsset();
        }

        _spriteRenderer.sprite = sprite;

        _characterPartsContainer.Index = index;
    }

    public bool IsValid()
    {
        return _spriteRenderer && _characterPartsContainer;
    }
}