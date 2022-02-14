#if UNITY_EDITOR || UNITY_STANDALONE
#define ENABLE_CONTROLS
#endif

using System.Diagnostics;
using UnityEngine;
using UnityEngine.Assertions;
#if UNITY_EDITOR
using UnityEditor;

#endif

public class Controls : MonoBehaviour
{
    [Conditional("ENABLE_CONTROLS")]
    void Update()
    {
        Assert.IsTrue(Application.isPlaying);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }
    }

    public static void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}