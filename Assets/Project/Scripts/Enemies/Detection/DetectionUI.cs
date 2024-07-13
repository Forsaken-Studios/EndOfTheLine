using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class DetectionUI : MonoBehaviour
{
    [SerializeField] private Image _detectionBarImage;
    [SerializeField] private Image _iconAlertState;
    [SerializeField] private Sprite _questionSprite;
    [SerializeField] private Sprite _detectedSprite;
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

    public void UpdateDetectionBar(float detectionLevel, bool isPlayerDetected)
    {
        _detectionBarImage.fillAmount = detectionLevel;
        _iconAlertState.sprite = isPlayerDetected ? _detectedSprite : _questionSprite;
    }
}
