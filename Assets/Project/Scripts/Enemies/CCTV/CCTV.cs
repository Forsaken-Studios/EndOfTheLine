using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTV : MonoBehaviour
{
    [SerializeField] private DetectionPlayerManager _basicEnemyDetection;
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private float _angle = 60f;

    private float _targetAngle;
    private float _currentAngle;
    private bool _increasing = true;

    void Start()
    {
        _targetAngle = _angle;
    }

    /// <summary>
    /// Movement from camera system
    /// We need to set MAX_ANGLE and MIN_ANGLE
    /// </summary>
    private void Update()
    {
        _currentAngle = transform.rotation.eulerAngles.z;

        if (_currentAngle > 180)
        {
            _currentAngle -= 360;
        }

        if (_increasing)
        {
            if (_currentAngle >= _angle)
            {
                _targetAngle = -_angle;
                _increasing = false;
            }
        }
        else
        {
            if (_currentAngle <= -_angle)
            {
                _targetAngle = _angle;
                _increasing = true;
            }
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, _targetAngle), _rotationSpeed * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        if (_basicEnemyDetection.currentState == EnemyStates.FOVState.isSeeing)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _basicEnemyDetection.GetMaxDistanceToNearEnemyPartner());
        }
    }
}
