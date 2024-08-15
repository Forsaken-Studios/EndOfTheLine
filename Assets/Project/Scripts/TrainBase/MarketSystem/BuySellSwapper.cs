using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuySellSwapper : MonoBehaviour
{
    [SerializeField] private GameObject buyPanel;
    [SerializeField] private GameObject sellPanel;
    
    [SerializeField] private Button buyPanelButton;
    [SerializeField] private Button sellPanelButton;


    private void OnEnable()
    {
        TrainManager.Instance.TrainStatus = TrainStatus.onMarketScreen;
        this.buyPanelButton.onClick.AddListener(() => SwapToBuyPanel());
        this.sellPanelButton.onClick.AddListener(() => SwapToSellPanel());
        this.buyPanel.SetActive(true);
        this.sellPanel.SetActive(false);
        this.sellPanelButton.interactable = true;
        this.buyPanelButton.interactable = false;
    }

    private void OnDisable()
    {
        TrainManager.Instance.TrainStatus = TrainStatus.onMarketRoom;
        TrainInventoryManager.Instance.CloseSellingInventory();
        TrainInventoryManager.Instance.LoadPlayerInventory();
        this.buyPanelButton.onClick.RemoveAllListeners();
        this.sellPanelButton.onClick.RemoveAllListeners();
    }

    private void SwapToSellPanel()
    {
        this.buyPanel.SetActive(false);
        this.sellPanel.SetActive(true);
        TrainInventoryManager.Instance.OpenInventoryStatusToSell();
        this.sellPanelButton.interactable = false;
        this.buyPanelButton.interactable = true;
    } 
    private void SwapToBuyPanel()
    {
        this.buyPanel.SetActive(true);
        this.sellPanel.SetActive(false);
        TrainInventoryManager.Instance.LoadPlayerInventory();
        this.sellPanelButton.interactable = true;
        this.buyPanelButton.interactable = false;
    }
}
