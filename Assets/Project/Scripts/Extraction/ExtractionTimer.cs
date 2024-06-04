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
        [Tooltip("Time needed for player to extract")]
        [SerializeField] private float timeToExtract = 10f;
        private float currentTime = 10f;
        private Animator _animator;

        private void Start()
        {
            currentTime = timeToExtract;
        }

        void Update()
        {
            if (currentTime <= 0)
            {
                if (ExtractionManager.Instance.GetIfExtractionArrived())
                {
                    //End game
                    GameManager.Instance.EndGame();
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
            currentTime = timeToExtract;
            StopAllCoroutines();
        }
    }
}
