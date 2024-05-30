using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Extraction
{ 
    public class ExtractionArriveTimer : MonoBehaviour
    {
        //Extraction Timer 
        [Header("UI Properties")]
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI extractionText;
        [Header("Timer properties")]
        [SerializeField] private float timeForTrainToArrive = 5f;
        [SerializeField] private float timeForTrainToLeave = 5f; 
        private float currentTime = 5f;
        private Animator _animator;
        private bool extractionIsLeaving = false;

        private void Start()
        {
            currentTime = timeForTrainToArrive;
        }

        void Update()
        {
            if (currentTime <= 0)
            {
                timerText.text = "0.00";
                if (!extractionIsLeaving)
                {
                    //Extraction arrived, we change extraction zone color, and enable player to extract
                    GameManager.Instance.ActivateExtractionZone();
                    ExtractionManager.Instance.ActivateExtractionTimeLeft();
                    extractionIsLeaving = true;
                  
                    StopAllCoroutines();
                    currentTime = timeForTrainToLeave;
                    extractionText.text = "Extraction will leave in: ";
                    StartCoroutine(StartCountdown());
                }
                else
                {
                    //Extraction is leaving, we reset all properties
                    extractionIsLeaving = false;
                    currentTime = timeForTrainToArrive;
                    GameManager.Instance.DesactivateExtractionZone();
                    extractionText.text = "Extraction will arrive in: ";
                    StopAllCoroutines();
                    this.gameObject.SetActive(false);
                }
                
                
            }
            
        }
        
        private void OnEnable()
        {
            _animator = GetComponent<Animator>();
            _animator.SetBool("extracting", true);
            //Start timer
            StartCoroutine(StartCountdown());
        }
        private IEnumerator StartCountdown()
        {
            while (true)
            {
                currentTime -= Time.deltaTime;
                timerText.text = currentTime.ToString("N");      
                yield return null;
            }
        }
        private void OnDisable()
        {
            _animator.SetBool("extracting", false);
            currentTime = timeForTrainToArrive;
            StopAllCoroutines();
        }
    } 
}

