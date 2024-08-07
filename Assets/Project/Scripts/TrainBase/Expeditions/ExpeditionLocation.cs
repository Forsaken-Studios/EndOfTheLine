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

    private void Start()
    { 
        
        buttonImage = GetComponentInChildren<Image>();
        stationButton = GetComponentInChildren<Button>();
        stationButton.onClick.AddListener(() => OnStationClicked());
        stationName.gameObject.SetActive(false);
        HideButton();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        stationName.gameObject.SetActive(true);
        ShowButton();
    }
    
    public void OnStationClicked()
    {
        //Change location in expeditionManager
        ExpeditionManager.Instance.ShowDetailsButton(expedition);
        isClicked = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        stationName.gameObject.SetActive(false);
        if(!isClicked)
            HideButton();
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


}
