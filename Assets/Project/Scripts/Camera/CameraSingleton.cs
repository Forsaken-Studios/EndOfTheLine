using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSingleton : MonoBehaviour
{
    private static CameraSingleton Instance;
    
    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("[CameraSingleton.cs] : There is already a CameraSingleton Instance");
            Destroy(this);
        }
        Instance = this;
    }
    
    public static CameraSingleton CameraSingletonInstance
    {
        get
        {
            return Instance;
        }
    }
}
