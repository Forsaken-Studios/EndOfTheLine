using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
   
    
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private FieldOfView enemyFOV;
    
    
    private bool _playerDetected;
    public bool PlayerIsDetected
    {
        get { return _playerDetected; }
        set { _playerDetected = value; }
    }
    
    private void Start()
    {
        
        //enemyFOV.SetAimDirection(new Vector3(360, 360, 0));
        enemyFOV.SetOrigin(this.gameObject.transform.position);
    }



    public FieldOfView GetFOV()
    {
        return enemyFOV;
    }
}
