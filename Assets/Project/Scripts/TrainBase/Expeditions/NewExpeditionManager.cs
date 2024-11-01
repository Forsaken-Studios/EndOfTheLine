using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;

public class NewExpeditionManager : MonoBehaviour
{
    public static NewExpeditionManager Instance;


     [Header("Expedition Reward Message")]
     [SerializeField] private GameObject detailsView;
     [SerializeField] private TextMeshProUGUI messageText;

     [Header("Mission Panel Prefab")] 
     [SerializeField] private GameObject missionPanelPrefab;
     [SerializeField] private GameObject missionsGrid;

     private MissionPanel missionPanelInProgress;
     private List<Button> missionsButtons;
     private MissionStatSO currentMissionSelected;
     [SerializeField] private Button startExpeditionButton;
     [SerializeField] private AStar aStarScript;
     private int lastExpeditionID = 1;
     private int currentExpeditionID;
     
     private void Awake()
     {
         if (Instance != null)
         {
             Debug.LogError("[NewExpeditionManager.cs] : There is already a NewExpeditionManager");
             Destroy(this);
         }

         Instance = this;
     }

     private void Start()
     {
         missionsButtons = new List<Button>();
         //Activate Message to receive rewards, if not, message to select expedition 
         HandleExpeditionRewardMessage();
         ShowAllMissionsInList();
     }
     

     private void HandleExpeditionRewardMessage()
     {
         int currentDayLocal = PlayerPrefs.GetInt("CurrentDay");
         int endingDay = PlayerPrefs.GetInt("ExpeditionEndDay");
         int isExpeditionInProgress = PlayerPrefs.GetInt("ExpeditionInProgress");
         messageText.text = (currentDayLocal >= endingDay && isExpeditionInProgress == 1)
             ? "Expedition Completed"
             : "Select a new expedition";
         
         detailsView.SetActive(false);
     }

     public MissionStatSO GetMission()
     {
         return currentMissionSelected;
     }
    

     private List<MissionStatSO> GetResourceMissionFromAllStation()
     {
         return UnityEngine.Resources.LoadAll<MissionStatSO>("Expedition").ToList();
     }


     
     private void ShowAllMissionsInList()
     {
         
         int currentDayLocal = PlayerPrefs.GetInt("CurrentDay");
         int endingDay = PlayerPrefs.GetInt("ExpeditionEndDay");
         int isExpeditionInProgress = PlayerPrefs.GetInt("ExpeditionInProgress");
         bool expeditionEnded = currentDayLocal >= endingDay && isExpeditionInProgress == 1;
         
         int expeditionID = PlayerPrefs.GetInt("ExpeditionID");
         List<MissionStatSO> allMissions = GetResourceMissionFromAllStation();
         foreach (var mission in allMissions)
         {
             GameObject missionPanel = Instantiate(missionPanelPrefab, Vector2.zero, Quaternion.identity, missionsGrid.transform);
             if (expeditionID == mission.id && expeditionEnded)
             {
                 this.missionPanelInProgress = missionPanel.GetComponent<MissionPanel>();
                 this.missionPanelInProgress.SetUpExpeditionTextToCompleted();
             }
             missionPanel.GetComponent<MissionPanel>().SetUpProperties(mission, expeditionID);
             
             Button button = missionPanel.GetComponent<Button>();
             missionsButtons.Add(button);
             if (expeditionEnded)
                 button.interactable = false;
         }
     }

     public void ResetDetailsView()
     {
         messageText.text = "Select a new expedition";
         startExpeditionButton.interactable = true;
         PlayerPrefs.SetInt("ExpeditionID", -1);
         foreach (var button in missionsButtons)
         {
             button.interactable = true;
         }
     }
     
     public void UpdateMissionPanelStatusToDisable()
     {
         if (this.missionPanelInProgress != null){
             this.missionPanelInProgress.HideExpeditionProgress();}

     }

     public void SetCurrentMissionPanelInProgress(MissionPanel panel)
     {
         this.missionPanelInProgress = panel;
     }

     public void ActivateDetailsView(MissionStatSO mission, MissionPanel missionPanel)
     {
         
         if (currentMissionSelected != null)
         {
             lastExpeditionID = currentMissionSelected.expeditionMapID;
         }

         if (currentMissionSelected != mission)
         {
             currentExpeditionID = mission.expeditionMapID;
             detailsView.GetComponent<ExpeditionDetailsPreview>().SetUpProperties(mission, missionPanel);
             currentMissionSelected = mission;
             detailsView.SetActive(true);
             aStarScript.SetCoordinates(lastExpeditionID, currentExpeditionID);
             aStarScript.RunPath();
             messageText.gameObject.SetActive(false);
         }
     }


     public int GetLastExpeditionID()
     {
         return lastExpeditionID;
     }
     public int GetCurrentExpeditionID()
     {
         return currentExpeditionID;
     }
}
