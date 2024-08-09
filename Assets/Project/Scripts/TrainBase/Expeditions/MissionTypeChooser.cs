using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionTypeChooser : MonoBehaviour
{
    [Header("Mission Type")]
    [SerializeField] private Button leftArrowButton;
    [SerializeField] private Button rightArrowButton;
    [SerializeField] private TextMeshProUGUI missionText;
    [Header("Chance of Success Text")]
    [SerializeField] private TextMeshProUGUI chanceOfSuccessText;

    [Header("Requirements Needed")] 
    [SerializeField] private GameObject requirementPrefab;
    [SerializeField] private GameObject requirementGrid;
    private List<GameObject> listOfRequirements;
    private float currentChanceOfSuccess;
    private int currentMissionIndex = 0;
    private List<MissionStatSO> missions;
    private MissionStatSO currentMissionSelected;
    

    private void OnEnable()
    {
        listOfRequirements = new List<GameObject>();
        missions = new List<MissionStatSO>();
        missions = ExpeditionManager.Instance.GetExpeditionClicked().GetMissions();
        leftArrowButton.onClick.AddListener(() => SwapMissionToLeft());
        rightArrowButton.onClick.AddListener(() => SwapMissionToRight());
        SetUpProperties(0);
        this.leftArrowButton.interactable = false;
        UpdateChanceOfSuccess(missions[currentMissionIndex].basicChanceOfSuccess);
    }

    private void OnDisable()
    {
        leftArrowButton.onClick.RemoveAllListeners();
        rightArrowButton.onClick.RemoveAllListeners();
    }

    private void SetUpProperties(int index)
    {
        UpdateChanceOfSuccess(missions[index].basicChanceOfSuccess);
       // Debug.Log(missions.Count);
        this.missionText.text = missions[index].missionName;
        currentMissionSelected = missions[index];
        //Set up requirements if needed
        SetUpRequirements();
    }

    private void SetUpRequirements()
    {

        foreach (var aux in listOfRequirements)
        {
            Destroy(aux);
        }
        listOfRequirements.Clear();

        string resourcesPath = "Expedition/MissionEXP" + ExpeditionManager.Instance.GetStationSelected().stationID + "/EXP" + 
                               (currentMissionIndex + 1) + "_MIS"  + (currentMissionIndex + 1) + "_Requirements";
        
        RequirementSO[] missionRequirements =
            UnityEngine.Resources.LoadAll<RequirementSO>(resourcesPath);

        if (missionRequirements.Length > 0)
        {
            foreach (var requirement in missionRequirements)
            {
                GameObject requirementGameObject = Instantiate(requirementPrefab, Vector2.zero, Quaternion.identity, requirementGrid.transform);
                listOfRequirements.Add(requirementGameObject);
                requirementGameObject.GetComponent<Requirement>().SetUpProperties(requirement); 
            }
        }
        else
        {
            //MESSAGE OF NO REQUIERMENT NEEDED
        }
        
 
    }
    
    private void SwapMissionToLeft()
    {
        int newIndex = currentMissionIndex - 1;
        currentMissionIndex = (int) Mathf.Clamp(newIndex, 0, this.missions.Count - 1);
        SetUpProperties(currentMissionIndex);
        BlockButtonIfFirstMission(currentMissionIndex);
    }
    
    private void SwapMissionToRight()
    {
        int newIndex = currentMissionIndex + 1;
        Debug.Log(newIndex);
        currentMissionIndex = Mathf.Clamp(newIndex, 0, this.missions.Count - 1);
        SetUpProperties(currentMissionIndex);
        BlockButtonIfLastMission(currentMissionIndex);
    }

    private void UpdateChanceOfSuccess(float value )
    {
        currentChanceOfSuccess = value;
        chanceOfSuccessText.text = value.ToString() + " %";
    }
    private void BlockButtonIfFirstMission(int index)
    {
        if (index == 0)
            this.leftArrowButton.interactable = false;
        
        if (index < missions.Count - 1)
            this.rightArrowButton.interactable = true;
    }
    private void BlockButtonIfLastMission(int index)
    {
        if (index > 0)
            this.leftArrowButton.interactable = true;
        
        if (index == missions.Count - 1)
            this.rightArrowButton.interactable = false;
    }
}
