using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Extraction
{       
    public class ExtractionTrigger : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                CheckIfWeCanExtractPlayers(other);
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                CheckIfWeCanExtractPlayers(other);
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (ExtractionManager.Instance.GetIfExtractionArrived())
                {
                    other.gameObject.GetComponent<PlayerManager>().SetIfPlayerInExtractionPoint(false);
                    ExtractionManager.Instance.StopExtraction();
                    ExtractionManager.Instance.SetPlayerInExtractionPoint(false);
                }
            }
        }


        private void CheckIfWeCanExtractPlayers(Collider2D other)
        {
            if (ExtractionManager.Instance.GetIfExtractionArrived())
            {
                other.gameObject.GetComponent<PlayerManager>().SetIfPlayerInExtractionPoint(true);
                ExtractionManager.Instance.SetPlayerInExtractionPoint(true);
                ExtractionManager.Instance.StartExtraction();
            }
        }
    }
}
