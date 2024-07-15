using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTV_Actions : MonoBehaviour
{
    [Header("Adjustable properties")]
    [SerializeField] private float _rotationSpeed = 15f;
    [SerializeField] private float _angle = 45f;

    [Header("External elements")]
    [SerializeField] private DetectionPlayerManager _basicEnemyDetection;

    [Header("Timer")]
    [SerializeField] private float timeToForget;
    private float _timer;

    private float _maxAngle;
    private float _minAngle;
    private float _currentAngle;
    private bool _increasing = true;

    void Start()
    {
        _currentAngle = transform.localEulerAngles.z;
        _maxAngle = _currentAngle + (_angle / 2);
        _minAngle = _currentAngle - (_angle / 2);

        Debug.Log("Max Angle: " + _maxAngle);
        Debug.Log("Min Angle: " + _minAngle);

        _timer = timeToForget;
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
        _currentAngle = transform.localEulerAngles.z;

        if (_increasing)
        {
            _currentAngle += _rotationSpeed * Time.deltaTime;
            if (_currentAngle >= _maxAngle)
            {
                _currentAngle = _maxAngle;
                _increasing = false;
            }
        }
        else
        {
            _currentAngle -= _rotationSpeed * Time.deltaTime;
            if (_currentAngle <= _minAngle)
            {
                _currentAngle = _minAngle;
                _increasing = true;
            }
        }

        transform.localEulerAngles = new Vector3(0, 0, _currentAngle);
    }

    void OnDrawGizmos()
    {
        if (_basicEnemyDetection != null && _basicEnemyDetection.currentState == EnemyStates.FOVState.isSeeing)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _basicEnemyDetection.GetMaxDistanceToNearEnemyPartner());
        }
    }
}
