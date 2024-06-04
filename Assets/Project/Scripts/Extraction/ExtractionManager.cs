using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extraction
{
    public class ExtractionManager : MonoBehaviour
    {
        public static ExtractionManager Instance;

        [Tooltip("GameObject with the text to make the animation")] [SerializeField]
        private GameObject extractionGameObject;

        [SerializeField] private GameObject extractionArriveGameObject;
        private Animator extractionAnimator;
        private bool playerInExtractionPoint = false;

        private bool extractionArrived = false;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.Log("[ExtractionManager] :: There is already a extractionManager");
                Destroy(this);
            }

            Instance = this;
        }

        void Start()
        {
            extractionAnimator = extractionGameObject.GetComponent<Animator>();
        }

        private void Update()
        {
            //TODO: Depende de si queremos movernos con el inventario abierto o no, deberíamos de poner onGame o != de pause
            if (Input.GetKeyDown(KeyCode.Z) && GameManager.Instance.GameState == GameState.OnGame)
            {
                //START COUNTDOWN FOR TRAIN TO ARRIVE
                StartExtractionArriveCountdown();
            }
        }

        public void StartExtractionArriveCountdown()
        {
            extractionArriveGameObject.SetActive(true);
        }


        public void StartExtraction()
        {
            if (extractionArrived)
            {
                SetPlayerInExtractionPoint(true);
                extractionGameObject.SetActive(true);
            }
        }

        public void StopExtraction()
        {
            try
            {
                if (extractionArrived)
                {
                    extractionGameObject.SetActive(false);
                    playerInExtractionPoint = false;
                }
            }
            catch (Exception e)
            {

            }
        }


        public void StopExtractionIfExtractionLeft()
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
    }
}
