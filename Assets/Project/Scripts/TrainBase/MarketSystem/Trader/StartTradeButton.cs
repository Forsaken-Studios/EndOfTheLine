using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StartTradeButton : MonoBehaviour
{

    private ItemTradeSO itemTrade;
    private int tradeID;
    private Button tradeButton;
    private bool receivingReward;
    public static event EventHandler<ItemTradeSO> onTradeStarted;
    public static event EventHandler onTradeEnded;
    
    public void SetUpProperties(ItemTradeSO item, int tradeID)
    {
        itemTrade = item;
        tradeButton = this.gameObject.GetComponent<Button>();
        tradeButton.onClick.AddListener(() => StartTrade());
        this.tradeID = this.tradeID;
    }


    public int GetTradeID()
    {
        return tradeID;
    }
    
    private void StartTrade()
    {
        if (receivingReward)
        {
            int id = PlayerPrefs.GetInt("TradeRewardID");
            Item item = TrainInventoryManager.Instance.GetItemFromID(id);

            if (TrainBaseInventory.Instance.TryAddItemCrateToItemSlot(item, 1, out int remainingItemsWithoutSpace))
            {
                PlayerPrefs.SetInt("TradeEndDay", 0);
                PlayerPrefs.SetInt("TradeRewardID", 0);
                PlayerPrefs.SetInt("TradeID", -1);
                PlayerPrefs.SetInt("TradeInProgress", 0);
                tradeButton.interactable = true;
                tradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "START";
                //Make rest of the buttons true again
                onTradeEnded?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                Debug.Log("NO SPACE FOR ITEM");
            } 
        }
        else
        {
            if (PlayerHasItemsNeeded())
            {
                foreach (var requirement in itemTrade.requirements)
                {
                    TrainBaseInventory.Instance.DeleteItemFromList(requirement.requirement.item, requirement.requirement.amountNeeded);
                    TrainBaseInventory.Instance.DeleteItemsFromItemSlot(requirement.requirement.item, requirement.requirement.amountNeeded);
                }
                tradeButton.interactable = false;
                tradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "IN PROGRESS";
                PlayerPrefs.SetInt("TradeID", GetTradeID());
                onTradeStarted?.Invoke(this, itemTrade); 
            }
        
        }
    }

    private bool PlayerHasItemsNeeded()
    {
        foreach (var requirement in itemTrade.requirements)
        {
            if (!TrainBaseInventory.Instance.GetIfItemIsInInventory(requirement.requirement.item,
                    requirement.requirement.amountNeeded))
            {
                return false; 
            }
        }

        return true;
    }

    public void SetReceivingReward(bool aux)
    {
        this.receivingReward = aux;
    }
}
