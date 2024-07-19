using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class DetectionUI : MonoBehaviour
{
    [Header("Bar progress")]
    [SerializeField] private Image _alertMeter;
    [SerializeField] private Gradient _colorGradient;
    [SerializeField] private float _detection;
    [SerializeField] private float _maxDetection;

    [Header("External elements")]
    [SerializeField] private Transform _bodyTransform;

    private Vector3 previousPosition;

    void Start()
    {
        previousPosition = _bodyTransform.position;
    }

    void Update()
    {
        UpdatePositionUI();
    }

    private void UpdatePositionUI()
    {
        Vector3 displacement = _bodyTransform.position - previousPosition;
        transform.position += displacement;
        previousPosition = _bodyTransform.position;
    }

    public void UpdateDetectionBar(float quantity)
    {
        _detection = Mathf.Clamp(quantity, 0, _maxDetection);
        _alertMeter.fillAmount = quantity;

        //Debug.Log(colorGradient.Evaluate(alertMeter.fillAmount));
        _alertMeter.color = _colorGradient.Evaluate(_alertMeter.fillAmount);
    }
}
