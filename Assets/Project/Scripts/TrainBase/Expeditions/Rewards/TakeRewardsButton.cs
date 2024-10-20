using System;
using System.Collections;
using System.Collections.Generic;
using LootSystem;
using SaveManagerNamespace;
using UnityEngine;
using UnityEngine.UI;

public class TakeRewardsButton : MonoBehaviour
{
    private Button takeRewardsButton;
    [Header("Expedition Failed prefab")] 
    [SerializeField] private GameObject expeditionFailed;
    [SerializeField] private GameObject expeditionSuccess;
    [SerializeField] private GameObject noSpaceForItemsMessage;


    
    private void OnEnable()
    {
        takeRewardsButton = GetComponentInChildren<Button>();
        if (takeRewardsButton != null)
        {
            takeRewardsButton.onClick.RemoveAllListeners();
            takeRewardsButton.onClick.AddListener(() => TakeRewards());
            int currentDayLocal = PlayerPrefs.GetInt("CurrentDay");
            int endingDay = PlayerPrefs.GetInt("ExpeditionEndDay");
            int isExpeditionInProgress = PlayerPrefs.GetInt("ExpeditionInProgress");
            takeRewardsButton.gameObject.SetActive(currentDayLocal >= endingDay && isExpeditionInProgress == 1);
        }
    }

  


    private void TakeRewards()
    {
        int currentDayLocal = PlayerPrefs.GetInt("CurrentDay");
        HandleExpeditionResult();
    }
    
    private void HandleExpeditionResult()
    {
        //Expedition ended
        int result = PlayerPrefs.GetInt("ExpeditionResult");
        if (result == 1)
        {
            ItemsDiccionarySave rewardsData = SaveManager.Instance.TryLoadExpeditionRewardJson();
            Dictionary<Item, int> items = TrainInventoryManager.Instance.GetItemsFromID(rewardsData.GetInventory());
            
            Dictionary<int, int> slotUsed = new Dictionary<int, int>();
            if (TrainBaseInventory.Instance.TryCheckIfThereIsSpaceForAllItems(items, out slotUsed))
            {
                GameObject expeditionSuccess= Instantiate(this.expeditionSuccess, Vector2.zero, Quaternion.identity);
                expeditionSuccess.GetComponentInChildren<ExpeditionSuccessPanel>().SetUpItemList(items);
                TrainManager.Instance.TrainStatus = TrainStatus.showingSpecialScreen;
                takeRewardsButton.gameObject.SetActive(false);
            }
            else
            {
                GameObject noSpaceForItems = Instantiate(this.noSpaceForItemsMessage, Vector2.zero, Quaternion.identity);
                foreach (var slot in slotUsed)
                {
                    TrainBaseInventory.Instance.RemoveItemFromItemSlot(slot.Key, slot.Value);
                    TrainManager.Instance.TrainStatus = TrainStatus.showingSpecialScreen;
                }
            }
        }
        else
        {
            GameObject expeditionFailed = Instantiate(this.expeditionFailed, Vector2.zero, Quaternion.identity);
            TrainManager.Instance.TrainStatus = TrainStatus.showingSpecialScreen;
            takeRewardsButton.gameObject.SetActive(false);
        }
        
    }
}
