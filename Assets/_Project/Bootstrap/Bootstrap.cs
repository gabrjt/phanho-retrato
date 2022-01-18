using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class Bootstrap
{
    static readonly CancellationTokenContainer CancellationTokenContainer = new();

    [RuntimeInitializeOnLoadMethod]
    static async void OnEnable()
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

#if UNITY_EDITOR
        sceneLoader.SetIndex();

        SceneManager.LoadScene(0);

        sceneLoader.ReloadScene();
#else
        sceneLoader.LoadNextScene();
#endif
    }
}