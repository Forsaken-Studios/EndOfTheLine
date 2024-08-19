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
     private ExpeditionLocation expeditionClicked;
     private MissionStatSO currentMissionSelected;
     [SerializeField] private GameObject detailsPrefab;
     private MissionTypeChooser missionChooser;
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
          TrainManager.Instance.AddScreenToList(details);
          details.GetComponentInChildren<DetailsView>().SetUpDetailsView(stationToShow);
     }
     
     public void ShowDetailsButton(ExpeditionSO expeditionSO, ExpeditionLocation expeditionLocation)
     {
          if(this.expeditionClicked != null)
               this.expeditionClicked .SetIfIsClicked(false);
          this.expeditionClicked = expeditionLocation;
          this.stationToShow = expeditionSO;
          TrainManager.Instance.TrainStatus = TrainStatus.usingWagon;
          detailsButton.gameObject.SetActive(true);
     }

     public void HideDetailsButton()
     {
          this.stationToShow = null;
          TrainManager.Instance.TrainStatus = TrainStatus.onExpeditionRoom;
          detailsButton.gameObject.SetActive(false);
     }

     public ExpeditionLocation GetExpeditionClicked()
     {
          return expeditionClicked;
     }

     public ExpeditionSO GetStationSelected()
     {
          return stationToShow;
     }

     public void SetMissionSelected(MissionStatSO mission)
     {
          this.currentMissionSelected = mission;
     }

     public MissionStatSO GetMission()
     {
          return currentMissionSelected;
     }

     public MissionTypeChooser GetMissionChooser()
     {
          return missionChooser;
     }

     public void SetMissionChooser(MissionTypeChooser mission)
     {
          this.missionChooser = mission;
     }
}
