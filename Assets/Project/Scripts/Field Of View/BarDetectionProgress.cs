using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils.CustomLogs;

namespace FieldOfView
{
    public class BarDetectionProgress : MonoBehaviour
    {

        [SerializeField] private Sprite questionSprite;
        [SerializeField] private Sprite detectedSprite;
        [SerializeField] private Image iconImage;
        private Animator _animator;
        private Image _image;
        private float speedBasedInDistance = 1f;
        private bool isDetecting = false;
        [SerializeField] private bool playerDetected = false;
        [SerializeField] private float detectionIncreaseFactor;
        [SerializeField] private float detectionDecreaseFactor;

        public event EventHandler onPlayerDetected;


        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _image = GetComponent<Image>();
        }

        void Start()
        {
            this.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!playerDetected)
            {
                CheckPlayerDetection();
            }
        }

        /// <summary>
        /// Method to fill or unfill the bar detection of enemies.
        /// </summary>
        private void CheckPlayerDetection()
        {
            if (isDetecting)
            {
                if (_image.fillAmount < 1)
                {
                    _image.fillAmount += detectionIncreaseFactor * Time.deltaTime;
                }
                else
                {
                    LogManager.Log("DETECTED", FeatureType.FieldOfView);
                    _image.fillAmount = 1;
                    playerDetected = true;
                    iconImage.sprite = detectedSprite;
                    _image.enabled = false;
                    isDetecting = false;
                }
            }
            else
            {
                if (_image.fillAmount == 0)
                {
                    isDetecting = false;
                    this.gameObject.SetActive(false);
                }

                if (_image.fillAmount >= 0)
                {
                    _image.fillAmount -= detectionIncreaseFactor * Time.deltaTime;
                }
                else
                {
                    _image.fillAmount = 0;
                }
            }
        }

        public void ForgetPlayer()
        {
            isDetecting = false;
            _image.fillAmount = 0;
            playerDetected = false;
            iconImage.sprite = questionSprite;
            _image.enabled = true;
        }

        private void OnEnable()
        {
            // this._animator.speed = speedBasedInDistance;
            isDetecting = true;
            iconImage.sprite = questionSprite;
        }

        private void OnDisable()
        {
            if (playerDetected)
            {

            }

            isDetecting = false;
        }

        public void SetIfPlayerIsBeingDetected(bool aux)
        {
            this.isDetecting = aux;
        }


        public void SetSpeedBasedInDistance(float distance)
        {
            float pctDistance = distance / 2.20f;
            speedBasedInDistance = Mathf.Lerp(pctDistance, 10, 100);
        }

        public bool GetIfPlayerIsDetected()
        {
            return playerDetected;
        }
    }
}
