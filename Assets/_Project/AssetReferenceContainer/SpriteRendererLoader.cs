using System;
using UnityEngine;

[Serializable]
public struct SpriteRendererLoader
{
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] AssetReferenceContainer _assetReferenceContainer;

    public void LoadNextSprite()
    {
        LoadAsset(_assetReferenceContainer.NextIndex);
    }

    public void LoadPreviousSprite()
    {
        LoadAsset(_assetReferenceContainer.PreviousIndex);
    }

    async void LoadAsset(int index)
    {
        if (_assetReferenceContainer.IsAssetReferenceValid(index))
        {
            return;
        }

        if (_assetReferenceContainer.TryUnloadCurrentAsset())
        {
            _spriteRenderer.sprite = null;
        }

        var (success, sprite) = await _assetReferenceContainer.LoadAsset<Sprite>(index);

        if (!success)
        {
            return;
        }

        _spriteRenderer.sprite = sprite;
    }

    public bool IsValid()
    {
        return _spriteRenderer && _assetReferenceContainer;
    }
}