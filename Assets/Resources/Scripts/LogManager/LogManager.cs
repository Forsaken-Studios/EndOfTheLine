using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.CustomLogs;

public class LogManager : MonoBehaviour
{
    public static LogManager Instance { get; private set; }

    private const string LOGS_PATH = "Assets/Resources/Scripts/LogManager/LogData.asset";
    private FeatureLogScriptable _logData;

    private void Awake()
    {       
        
        if (Instance != null)
        {
            Debug.LogError("There's more than one LogManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        LoadDebugConfig();
    }

    private void LoadDebugConfig()
    {
#if UNITY_EDITOR
        _logData = (FeatureLogScriptable) UnityEditor.AssetDatabase.LoadAssetAtPath(LOGS_PATH, typeof(FeatureLogScriptable));

        // TODO: THROW LOG IF NULL
#endif
    }

    private void LogConsole(string txt, FeatureType feature)
    {
        var logData = _logData.GetLogData(feature);
        
        if (logData != null && logData.Enabled)
        {
            var hexColor = ToHex(logData.CustomColor);
            string header = $"[<color={hexColor}> <b>{feature}</b>:</color>]";
            Debug.Log($"{header} {txt}");
        }
        else
        {
            Debug.Log("LOG NOT WORKING PROPERTLY");
        }
    }

    private string ToHex(Color color)
    {
        var col = (Color32)color;
        return $"#{col.r.ToString("x2")}{col.g.ToString("x2")}{col.b.ToString("x2")}{col.a.ToString("x2")}";
    }


    public static void Log(string text, FeatureType feature)
    {
#if UNITY_EDITOR
        Instance?.LogConsole(text, feature);
#endif
    }

}
