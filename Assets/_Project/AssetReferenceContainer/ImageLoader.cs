using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ImageLoader
{
    [SerializeField] [Required] Image _image;
    [SerializeField] [Required] [Expandable] AssetReferenceContainer _assetReferenceContainer;

    public void LoadNextImage()
    {
        LoadAsset(_assetReferenceContainer.NextIndex);
    }

    public void LoadPreviousImage()
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
            _image.sprite = null;
        }

        var (success, sprite) = await _assetReferenceContainer.LoadAsset<Sprite>(index);

        if (!success)
        {
            return;
        }

        _image.sprite = sprite;
    }

    public bool IsValid()
    {
        return _image && _assetReferenceContainer;
    }
}