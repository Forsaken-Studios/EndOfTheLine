using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExpeditionLocation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField] private TextMeshProUGUI stationName;
    [SerializeField] private ExpeditionSO expedition;
    private Image buttonImage;
    private Button stationButton;
    private bool isClicked;

    private List<MissionStatSO> stationMissions;
    
    private void Start()
    {
        stationMissions = new List<MissionStatSO>();
        buttonImage = GetComponentInChildren<Image>();
        stationButton = GetComponentInChildren<Button>();
        stationButton.onClick.AddListener(() => OnStationClicked());
        stationName.gameObject.SetActive(false);
        GetResourceMissionFromStation();
        HideButton();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        stationName.gameObject.SetActive(true);
        ShowButton();
    }
    
    public void OnStationClicked()
    {
        //UnClick previous station
        if (ExpeditionManager.Instance.GetExpeditionClicked() != null && ExpeditionManager.Instance.GetExpeditionClicked() != this)
        {
            ExpeditionManager.Instance.GetExpeditionClicked().stationName.gameObject.SetActive(false);
            ExpeditionManager.Instance.GetExpeditionClicked().HideButton();
        }
        //Change location in expeditionManager
        ExpeditionManager.Instance.ShowDetailsButton(expedition, this);
        isClicked = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isClicked)
        {
            stationName.gameObject.SetActive(false);
            HideButton();
        }
    }

    private void HideButton()
    {
        var tempColor = buttonImage.color;
        tempColor.a = 0f;
        buttonImage.color = tempColor;
    }

    private void ShowButton()
    {
        var tempColor = buttonImage.color;
        tempColor.a = 1f;
        buttonImage.color = tempColor;
    }

    public List<MissionStatSO> GetMissions()
    {
        return stationMissions;
    }
    
    private void GetResourceMissionFromStation()
    {
        UnityEngine.Object[] missions = UnityEngine.Resources.LoadAll<MissionStatSO>("Expedition/MissionEXP" + expedition.stationID);
        MissionObjectToList(missions);
    }

    private void MissionObjectToList(UnityEngine.Object[] missions) 
    {
        foreach (var mission in missions)
        {
            stationMissions.Add(mission as MissionStatSO);
        }
    }
    
}
