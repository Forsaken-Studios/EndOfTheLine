using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Extraction
{
    public class ExtractionManager : MonoBehaviour
    {
        public static ExtractionManager Instance;

        [Tooltip("GameObject with the text to make the animation")]
        private GameObject currentTimeLeftToExtractGameObject;

        private GameObject extractionTimeLeftToArrive;
        private Animator extractionAnimator;
        private bool playerInExtractionPoint = false;

        private bool extractionArrived = false;
        [Header("Raid Time")]
        [SerializeField] private float raidTime;
        [field: Tooltip("Time needed for player to extract")]
        [Header("Time need To Extract")]
        [SerializeField] private float timeToExtract;
        public float TimeToExtract { get => timeToExtract; }
        [field: Header("Timer properties")]
        [SerializeField] private float timeForTrainToArrive;
        public float TimeForTrainToArrive { get => timeForTrainToArrive; }
        [SerializeField] private float timeForTrainToLeave;
        public float TimeForTrainToLeave { get => timeForTrainToLeave; }

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.Log("[ExtractionManager] :: There is already a extractionManager");
                Destroy(this);
            }

            Instance = this;
       
        }

        private void Start()
        {
            GetReferences();
        }

        private void GetReferences()
        {
            currentTimeLeftToExtractGameObject =
                GameManager.Instance.GetMenuCanvas().transform.Find("Extraction/CurrentTimeLeftToExtractGameObject").gameObject;
            extractionTimeLeftToArrive =
                GameManager.Instance.GetMenuCanvas().transform.Find("Extraction/ExtractionTimeLeftToArrive").gameObject;
            extractionAnimator = currentTimeLeftToExtractGameObject.GetComponent<Animator>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z) && GameManager.Instance.GameState == GameState.OnGame)
            {
                //START COUNTDOWN FOR TRAIN TO ARRIVE
                StartExtractionArriveCountdown();
            }
        }

        public void StartExtractionArriveCountdown()
        {
            extractionTimeLeftToArrive.SetActive(true);
        }


        public void StartExtraction()
        {
            if (extractionArrived)
            {
                SetPlayerInExtractionPoint(true);
                currentTimeLeftToExtractGameObject.SetActive(true);
            }
        }

        public void StopExtraction()
        {
            try
            {
                if (extractionArrived)
                {
                    currentTimeLeftToExtractGameObject.SetActive(false);
                    playerInExtractionPoint = false;
                }
            }
            catch (Exception e)
            {

            }
        }


        public void StopExtractionIfTrainLeft()
        {
            if (playerInExtractionPoint)
            {
                this.StopExtraction();
            }
        }

        public bool GetIfPlayerIsInExtractionPoint()
        {
            return playerInExtractionPoint;
        }

        public void SetPlayerInExtractionPoint(bool aux)
        {
            this.playerInExtractionPoint = aux;
        }

        public bool GetIfExtractionArrived()
        {
            return extractionArrived;
        }

        public void SetIfExtractionArrived(bool aux)
        {
            this.extractionArrived = aux;
        }

        public void ActivateExtractionTimeLeft()
        {

        }

        public float GetRaidTime()
        {
            return raidTime;
        }
    }
}
