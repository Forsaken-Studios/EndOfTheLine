using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtractionTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (ExtractionManager.Instance.GetIfExtractionArrived())
            {
                other.gameObject.GetComponent<PlayerManager>().SetIfPlayerInExtractionPoint(true);
                ExtractionManager.Instance.SetPlayerInExtractionPoint(true);
                ExtractionManager.Instance.StartExtraction(); 
            }
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
}
