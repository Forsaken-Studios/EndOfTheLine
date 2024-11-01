using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuySellSwapper : MonoBehaviour
{
    [SerializeField] private GameObject buyPanel;
    [SerializeField] private GameObject sellPanel;
    [SerializeField] private GameObject tradePanel;
    
    [SerializeField] private Button buyPanelButton;
    [SerializeField] private Button sellPanelButton;
    [SerializeField] private Button tradePanelButton;

    private bool isSelling = false;

    private void OnEnable()
    {
        TrainManager.Instance.TrainStatus = TrainStatus.onMarketScreen;
        this.buyPanelButton.onClick.AddListener(() => SwapToBuyPanel());
        this.sellPanelButton.onClick.AddListener(() => SwapToSellPanel());
        this.tradePanelButton.onClick.AddListener(() => SwapToTradePanel());
        this.buyPanel.SetActive(true);
        isSelling = false;
        this.sellPanel.SetActive(false);
        this.tradePanel.SetActive(false);
        this.sellPanelButton.interactable = true;
        this.tradePanelButton.interactable = true;
        this.buyPanelButton.interactable = false;
    }

    private void OnDisable()
    {
        TrainManager.Instance.TrainStatus = TrainStatus.onMarketRoom;
        if (isSelling)
        {
            TrainInventoryManager.Instance.CloseSellingInventory();
            TrainInventoryManager.Instance.LoadPlayerInventory();
        }
        this.buyPanelButton.onClick.RemoveAllListeners();
        this.sellPanelButton.onClick.RemoveAllListeners();
        this.tradePanelButton.onClick.RemoveAllListeners();
        
    }

    private void SwapToSellPanel()
    {
        this.buyPanel.SetActive(false);
        this.tradePanel.SetActive(false);
        this.sellPanel.SetActive(true);
        isSelling = true;
        TrainInventoryManager.Instance.OpenInventoryStatusToSell();
        this.sellPanelButton.interactable = false;
        this.buyPanelButton.interactable = true;
        this.tradePanelButton.interactable = true;
    } 
    private void SwapToBuyPanel()
    {
        if (isSelling)
        {
            TrainInventoryManager.Instance.CloseSellingInventory();
            TrainInventoryManager.Instance.LoadPlayerInventory();
            isSelling = false;
        }
        this.buyPanel.SetActive(true);  
        this.tradePanel.SetActive(false);
        this.sellPanel.SetActive(false);

        this.sellPanelButton.interactable = true;
        this.tradePanelButton.interactable = true;
        this.buyPanelButton.interactable = false;
    }  
    private void SwapToTradePanel()
    {
        if (isSelling)
        {
            TrainInventoryManager.Instance.CloseSellingInventory();
            TrainInventoryManager.Instance.LoadPlayerInventory();
            isSelling = false;
        }
        this.buyPanel.SetActive(false);
        this.tradePanel.SetActive(true);
        this.sellPanel.SetActive(false);

        this.sellPanelButton.interactable = true;
        this.tradePanelButton.interactable = false;
        this.buyPanelButton.interactable = true;
    }
}
