using System.Diagnostics;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class Bootstrap
{
    static readonly CancellationTokenContainer CancellationTokenContainer = new();

    [Conditional("UNITY_EDITOR")]
    [RuntimeInitializeOnLoadMethod]
    static async void Blabber()
    {
        Application.wantsToQuit += () =>
        {
            CancellationTokenContainer.Cancel();
            return true;
        };

        var (cancelled, sceneLoader) = await Addressables.LoadAssetAsync<SceneLoader>("Scene Loader").WithCancellation(CancellationTokenContainer.CancellationToken).SuppressCancellationThrow();

        if (cancelled)
        {
            return;
        }

        sceneLoader.SetIndex();

        SceneManager.LoadScene(0);

        sceneLoader.ReloadScene();
    }
}