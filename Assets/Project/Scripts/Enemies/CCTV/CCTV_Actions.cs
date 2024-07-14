using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTV_Actions : MonoBehaviour
{
    [Header("Adjustable properties")]
    [SerializeField] private float _rotationSpeed = 15f;
    [SerializeField] private float _angle = 45f;
    private float _realAngle;

    [Header("Exernal elements")]
    [SerializeField] private DetectionPlayerManager _basicEnemyDetection;

    [Header("Timer")]
    [SerializeField] private float timeToForget;
    private float _timer;

    private float _targetAngle;
    private float _currentAngle;
    private bool _increasing = true;

    void Start()
    {
        _realAngle = _angle / 2;
        _timer = timeToForget;
        _targetAngle = _angle;
    }

    /// <summary>
    /// Movement from camera system
    /// We need to set MAX_ANGLE and MIN_ANGLE
    /// </summary>
    private void Update()
    {
        RotateCamera();
        Timer();
    }

    private void Timer()
    {
        if (_basicEnemyDetection.isPlayerDetected)
        {
            if (_basicEnemyDetection.currentState == EnemyStates.FOVState.isSeeing)
            {
                _timer = timeToForget;
            }
            else
            {
                _timer -= Time.deltaTime;

                if (_timer <= 0)
                {
                    _basicEnemyDetection.StopDetection();
                }
            }
        }
    }

    private void RotateCamera()
    {
        _currentAngle = transform.rotation.eulerAngles.z;

        if (_currentAngle > 180)
        {
            _currentAngle -= 360;
        }

        if (_increasing)
        {
            if (_currentAngle >= _realAngle)
            {
                _targetAngle = -_realAngle;
                _increasing = false;
            }
        }
        else
        {
            if (_currentAngle <= -_realAngle)
            {
                _targetAngle = _realAngle;
                _increasing = true;
            }
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, _targetAngle), _rotationSpeed * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        if (_basicEnemyDetection.currentState == EnemyStates.FOVState.isSeeing)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _basicEnemyDetection.GetMaxDistanceToNearEnemyPartner());
        }
    }
}
