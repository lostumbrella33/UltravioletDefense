using System.Collections;
using UnityEngine;

public class StaticCoroutineRunner : MonoBehaviour
{
    private static StaticCoroutineRunner instance;

    private static void EnsureInstance()
    {
        if (instance == null)
        {
            GameObject runner = new GameObject("StaticCoroutineRunner");
            instance = runner.AddComponent<StaticCoroutineRunner>();
            DontDestroyOnLoad(runner);
        }
    }

    public static void StartStaticCoroutine(IEnumerator routine)
    {
        EnsureInstance();
        instance.StartCoroutine(routine);
    }
}
