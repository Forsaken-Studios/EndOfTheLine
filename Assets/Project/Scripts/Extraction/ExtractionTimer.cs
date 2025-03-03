using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Extraction
{
    public class ExtractionTimer : MonoBehaviour
    {
        //Extraction Timer 
        [Header("Extraction timer")]
        [SerializeField] private TextMeshProUGUI timerText;
        [Header("Extraction Time")]

        private float currentTime = 10f;
        private Animator _animator;

        private bool isEnding = false;
        private void Start()
        {
            currentTime = ExtractionManager.Instance.TimeToExtract;
        }

        void Update()
        {
            if (currentTime <= 0)
            {
                timerText.text = "0.00";
                if (ExtractionManager.Instance.GetIfExtractionArrived())
                {
                    //End game
                    if (!isEnding)
                    {
                        GameManager.Instance.EndGame(false);
                        isEnding = true;
                    }
                    
                }
                StopAllCoroutines();
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
            currentTime = ExtractionManager.Instance.TimeToExtract;;
            StopAllCoroutines();
        }
    }
}
