using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSingleton : MonoBehaviour
{
    private static CameraSingleton Instance;

    [SerializeField] private float inventoryZoomValue = 2; 
    [SerializeField] private float normalZoomValue = 5;
    [SerializeField] private float zoomLerp = 0.02f;
    [SerializeField] private bool zoomIn;
    [SerializeField] private bool zoomOut;
    
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

    private void Update()
    {
        if (zoomIn)
        {
            float currentValue = this.GetComponent<Camera>().orthographicSize;
            this.GetComponent<Camera>().orthographicSize = Mathf.Lerp(currentValue, inventoryZoomValue, zoomLerp);
            if (currentValue == inventoryZoomValue)
            {
                zoomIn = false;
            }
        }else if (zoomOut)
        {
            float currentValue = this.GetComponent<Camera>().orthographicSize;
            this.GetComponent<Camera>().orthographicSize = Mathf.Lerp(currentValue, normalZoomValue, zoomLerp);
            if (currentValue == normalZoomValue)
            {
                zoomOut = false;
            }
        }
    }

    public void ZoomCameraOnInventory()
    {
        zoomOut = false;
        zoomIn = true;
    }
    
    public void UnZoomToNormalPosition()
    {
        zoomIn = false;
        zoomOut = true;
    }
}
