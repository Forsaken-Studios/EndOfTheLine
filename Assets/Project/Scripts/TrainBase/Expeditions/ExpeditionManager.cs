using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpeditionManager : MonoBehaviour
{
     public static ExpeditionManager Instance;

     [SerializeField] private Button detailsButton;
     private ExpeditionSO stationToShow;
     [SerializeField] private GameObject detailsPrefab;
     private void Awake()
     {
          if (Instance != null)
          {
               Debug.LogWarning("[GameManager.cs] : There is already a ExpeditionManager Instance");
               Destroy(this);
          }
          Instance = this;
     }


     private void Start()
     {
         HideDetailsButton(); 
         detailsButton.onClick.AddListener(() => ShowDetails());
     }

     private void ShowDetails()
     {
          GameObject details = Instantiate(detailsPrefab, Vector2.zero, Quaternion.identity);
          
     }
     
     public void ShowDetailsButton(ExpeditionSO expeditionSO)
     {
          this.stationToShow = expeditionSO;
          Debug.Log("EXPEDITION IN: " + expeditionSO.stationName);
          TrainManager.Instance.TrainStatus = TrainStatus.usingWagon;
          detailsButton.gameObject.SetActive(true);
     }

     public void HideDetailsButton()
     {
          this.stationToShow = null;
          TrainManager.Instance.TrainStatus = TrainStatus.onExpeditionRoom;
          detailsButton.gameObject.SetActive(false);
     }
}
