using System;
using System.Collections;
using System.Collections.Generic;
using LootSystem;
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
        this.tradeID = tradeID;
    }


    public int GetTradeID()
    {
        return tradeID;
    }
    
    private void StartTrade()
    {
            if (PlayerHasItemsNeeded())
            {
                Item item = itemTrade.itemReceived.item.itemReceived;
                if (TrainBaseInventory.Instance.TryAddItemCrateToItemSlot(item, 1, out int remainingItemsWithoutSpace))
                {
                    foreach (var requirement in itemTrade.requirements)
                    {
                        TrainBaseInventory.Instance.DeleteItemFromList(requirement.requirement.item, requirement.requirement.amountNeeded);
                        TrainBaseInventory.Instance.FindAndDeleteItemsFromItemSlot(requirement.requirement.item, requirement.requirement.amountNeeded);
                    }
                    tradeButton.interactable = false;
                    tradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "NO STOCK";
                    PlayerPrefs.SetInt("TradeID_" + itemTrade.id, itemTrade.id);
                   // onTradeStarted?.Invoke(this, itemTrade); 
                    tradeButton.interactable = false;
                    
                    //tradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "START";
                    //Make rest of the buttons true again
                   // onTradeEnded?.Invoke(this, EventArgs.Empty);
                }else
                {
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
