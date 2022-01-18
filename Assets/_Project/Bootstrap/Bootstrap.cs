using System.Runtime.CompilerServices;
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

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            sceneLoader.Reset();
            sceneLoader.LoadNextScene();

            return;
        }

        LoadGameFlow(sceneLoader);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void LoadGameFlow(SceneLoader sceneLoader)
    {
#if UNITY_EDITOR
        if (!sceneLoader.TrySetIndex())
        {
            return;
        }

        SceneManager.LoadScene(0);

        sceneLoader.ReloadScene();
#endif
    }
}