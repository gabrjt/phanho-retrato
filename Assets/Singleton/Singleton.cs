using UnityEngine;
using UnityEngine.Assertions;

public abstract class Singleton<T> : MonoBehaviour
    where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    void OnEnable()
    {
        Assert.IsNull(Instance);

        Instance = GetComponent<T>();

        Assert.IsNotNull(Instance);

        DontDestroyOnLoad(gameObject);
    }

    void OnDisable()
    {
        Assert.IsNotNull(Instance);
        Assert.AreEqual(Instance, GetComponent<T>());

        Instance = null;

        Destroy(gameObject);
    }
}