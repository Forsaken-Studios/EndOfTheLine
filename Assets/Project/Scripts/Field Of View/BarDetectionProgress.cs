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
        
        private Animator _animator;
        private Image _image;
        private float speedBasedInDistance = 1f;
        private bool isDetecting = false;
        [SerializeField] private bool playerDetected = false;
        [SerializeField] private float detectionIncreaseFactor;
        [SerializeField] private float detectionDecreaseFactor;
        private EnemyFOVState enemyFOVState;
        public event EventHandler onPlayerDetected;
        private AlertColorHUD alertColorHUD;
        [SerializeField] private float detectionValue;
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _image = GetComponent<Image>();
        }

        void Start()
        {
            this.gameObject.SetActive(false);
            alertColorHUD = GetComponent<AlertColorHUD>();
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
                if (detectionValue < 1)
                {
                    detectionValue += detectionIncreaseFactor * Time.deltaTime;
                }
                else
                {
                    LogManager.Log("DETECTED", FeatureType.FieldOfView);
                    GetComponentInParent<Enemy>().PlayerDetected = true; 
                    enemyFOVState.FOVState = FOVState.isSeeing;
                    detectionValue = 1;
                    playerDetected = true; 
                    //_image.enabled = false;
                    isDetecting = false;
                }
            }
            else
            {
                if (detectionValue == 0)
                {
                    isDetecting = false;
                    this.gameObject.SetActive(false);
                }

                if (detectionValue >= 0)
                {
                    Debug.Log("DECREASE ");
                   detectionValue -= detectionIncreaseFactor * Time.deltaTime;
                }
                else
                {
                    detectionValue = 0;
                }
            }
            
            alertColorHUD.UpdateHealth(detectionValue);
        }

        public void ForgetPlayer()
        {
            isDetecting = false;
            detectionValue = 0;
            playerDetected = false;
            _image.enabled = true;
        }

        private void OnEnable()
        {
            // this._animator.speed = speedBasedInDistance;
            isDetecting = true;
        }

        private void OnDisable()
        {
            if (playerDetected)
            {

            }

            isDetecting = false;
        }

        public void SetIfPlayerIsBeingDetected(bool aux, EnemyFOVState enemyFOVState)
        {
            this.enemyFOVState = enemyFOVState;
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
