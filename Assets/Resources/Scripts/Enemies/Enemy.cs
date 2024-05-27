using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private bool _playerDetected;
    public bool PlayerIsDetected
    {
        get { return _playerDetected; }
        set { _playerDetected = value; }
    }
    
}
