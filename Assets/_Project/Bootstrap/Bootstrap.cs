using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
#endif

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
            sceneLoader.LoadFirstScene();

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

#if UNITY_EDITOR
    public static void SetExpanded(Scene scene, bool expand)
    {
        foreach (var window in Resources.FindObjectsOfTypeAll<SearchableEditorWindow>())
        {
            if (window.GetType().Name != "SceneHierarchyWindow")
            {
                continue;
            }

            var method = window.GetType().GetMethod("SetExpandedRecursive", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new[] {typeof(int), typeof(bool)}, null);

            if (method == null)
            {
                Debug.LogError("Could not find method 'UnityEditor.SceneHierarchyWindow.SetExpandedRecursive(int, bool)'.");
                return;
            }

            var field = scene.GetType().GetField("m_Handle", BindingFlags.NonPublic | BindingFlags.Instance);

            if (field == null)
            {
                Debug.LogError("Could not find field 'int UnityEngine.SceneManagement.Scene.m_Handle'.");
                return;
            }

            var sceneHandle = field.GetValue(scene);
            method.Invoke(window, new[] {sceneHandle, expand});
        }
    }
#endif
}