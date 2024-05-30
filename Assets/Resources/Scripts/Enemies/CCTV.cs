using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTV : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 5f;
    private FieldOfView.FieldOfView cctvFOV; 
    
    [SerializeField] private float MAX_ANGLE = 180;
    [SerializeField] private float MIN_ANGLE = 120;
    
    private bool increasing = true;
    private void Start()
    {
        cctvFOV = GetComponent<Enemy>().GetFOV();
    }

    /// <summary>
    /// Movement from camera system
    /// We need to set MAX_ANGLE and MIN_ANGLE
    /// </summary>
    private void Update()
    {
        if (increasing)
        {
            if (cctvFOV.GetAngle() >= MAX_ANGLE)
                increasing = false;
            
            cctvFOV.SetAngle(cctvFOV.GetAngle() + rotationSpeed * Time.deltaTime);
        }
        else
        {
            if (cctvFOV.GetAngle() <= MIN_ANGLE)
                increasing = true;
            
            cctvFOV.SetAngle(cctvFOV.GetAngle() - rotationSpeed * Time.deltaTime);
        }
    }

    
    
    
}
