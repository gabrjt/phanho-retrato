using System;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;

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
        Assert.IsTrue(Application.isPlaying);
        Assert.IsTrue(_characterPartsContainer.IsValidIndex(index));

        _characterPartsContainer.Cancel();

        var assetReference = _characterPartsContainer[index];

        var (cancelled, sprite) = await assetReference.LoadAssetAsync<Sprite>().WithCancellation(_characterPartsContainer.CancellationToken).SuppressCancellationThrow();

        if (cancelled)
        {
            return;
        }

        _spriteRenderer.sprite = null;

        TryUnloadCurrentAsset();

        _spriteRenderer.sprite = sprite;

        _characterPartsContainer.Index = index;
    }

    bool TryUnloadCurrentAsset()
    {
        if (!_characterPartsContainer.IsValidIndex())
        {
            return false;
        }

        var assetReference = _characterPartsContainer.Current;

        if (!assetReference.IsValid())
        {
            return false;
        }

        assetReference.ReleaseAsset();

        return true;
    }

    public bool IsValid()
    {
        return _spriteRenderer && _characterPartsContainer;
    }
}