using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MechanicPanelSwapper : MonoBehaviour
{
    [SerializeField] private GameObject upgradesPanel;
    [SerializeField] private GameObject abilityShopPanel;
    [SerializeField] private GameObject abilityEquipmentPanel;
    
    [SerializeField] private Button upgradesButton;
    [SerializeField] private Button abilityShopButton;
    [SerializeField] private Button abilityEquipmentButton;

    private bool isSelling = false;

    private void OnEnable()
    {
        TrainManager.Instance.TrainStatus = TrainStatus.onMechanicScreen;
        this.upgradesButton.onClick.AddListener(() => SwapToUpgradesPanel());
        this.abilityShopButton.onClick.AddListener(() => SwapToMechanicPanel());
        this.abilityEquipmentButton.onClick.AddListener(() => SwapToEquipmentPanel());
        this.upgradesPanel.SetActive(true);
        isSelling = false;
        this.abilityShopPanel.SetActive(false);
        this.abilityEquipmentPanel.SetActive(false);
        this.abilityShopButton.interactable = true;
        this.abilityEquipmentButton.interactable = true;
        this.upgradesButton.interactable = false;
    }

    private void OnDisable()
    {
        TrainManager.Instance.TrainStatus = TrainStatus.onMechanicRoom;
        this.upgradesButton.onClick.RemoveAllListeners();
        this.abilityShopButton.onClick.RemoveAllListeners();
        this.abilityEquipmentButton.onClick.RemoveAllListeners();
    }

    private void SwapToMechanicPanel()
    {
        this.upgradesPanel.SetActive(false);
        this.abilityEquipmentPanel.SetActive(false);
        this.abilityShopPanel.SetActive(true);
        isSelling = true;
        this.abilityShopButton.interactable = false;
        this.upgradesButton.interactable = true;
        this.abilityEquipmentButton.interactable = true;
    } 
    private void SwapToUpgradesPanel()
    {
        this.upgradesPanel.SetActive(true);  
        this.abilityEquipmentPanel.SetActive(false);
        this.abilityShopPanel.SetActive(false);

        this.abilityShopButton.interactable = true;
        this.abilityEquipmentButton.interactable = true;
        this.upgradesButton.interactable = false;
    }  
    private void SwapToEquipmentPanel()
    {
        this.upgradesPanel.SetActive(false);
        this.abilityEquipmentPanel.SetActive(true);
        this.abilityShopPanel.SetActive(false);

        this.abilityShopButton.interactable = true;
        this.abilityEquipmentButton.interactable = false;
        this.upgradesButton.interactable = true;
    }
}
