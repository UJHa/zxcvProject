using UnityEditor;
using UnityEngine;

public class ReleaseLog : MonoBehaviour
{
    public static bool enable = true;
    public static void LogInfo(string log)
    {
        if (enable)
            Debug.Log($"{log}");
    }

    public static void LogError(string log)
    {
        if (enable)
            Debug.Log($"{log}");
    }
}