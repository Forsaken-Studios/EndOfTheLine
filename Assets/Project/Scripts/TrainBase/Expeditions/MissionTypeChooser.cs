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
    private float currentChanceOfSuccess;
    private int currentMissionIndex = 0;
    private List<MissionStatSO> missions;
    private MissionStatSO currentMissionSelected;
    

    private void OnEnable()
    {
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
        Debug.Log("COUNT " + missions.Count);
        Debug.Log(missions[index]);
        Debug.Log("INDEX: " + index);
       // Debug.Log(missions.Count);
        this.missionText.text = missions[index].missionName;
        currentMissionSelected = missions[index];
        //Set up requirements if needed
    }
    
    private void SwapMissionToLeft()
    {
        int newIndex = currentMissionIndex - 1;
        currentMissionIndex = (int) Mathf.Clamp(newIndex, 0, this.missions.Count - 1);
        SetUpProperties(currentMissionIndex);
        BlockButtonIfFirstMission(currentMissionIndex);
        UpdateChanceOfSuccess(missions[currentMissionIndex].basicChanceOfSuccess);
    }
    
    private void SwapMissionToRight()
    {
        int newIndex = currentMissionIndex + 1;
        Debug.Log(newIndex);
        currentMissionIndex = Mathf.Clamp(newIndex, 0, this.missions.Count - 1);
        SetUpProperties(currentMissionIndex);
        BlockButtonIfLastMission(currentMissionIndex);
        UpdateChanceOfSuccess(missions[currentMissionIndex].basicChanceOfSuccess);
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
