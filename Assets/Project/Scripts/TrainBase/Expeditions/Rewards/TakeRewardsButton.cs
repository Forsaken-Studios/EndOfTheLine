using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TakeRewardsButton : MonoBehaviour
{
    private Button takeRewardsButton;
    [Header("Expedition Failed prefab")] 
    [SerializeField] private GameObject expeditionFailed;
    [SerializeField] private GameObject expeditionSuccess;


    
    private void OnEnable()
    {
        takeRewardsButton = GetComponentInChildren<Button>();
        
        takeRewardsButton.onClick.AddListener(() => TakeRewards() );
        
        int currentDayLocal = PlayerPrefs.GetInt("CurrentDay");
        int endingDay = PlayerPrefs.GetInt("ExpeditionEndDay");
        int isExpeditionInProgress = PlayerPrefs.GetInt("ExpeditionInProgress");
        takeRewardsButton.gameObject.SetActive(currentDayLocal >= endingDay && isExpeditionInProgress == 1);
    }


    private void TakeRewards()
    {
        int currentDayLocal = PlayerPrefs.GetInt("CurrentDay");
        HandleExpeditionResult(currentDayLocal);
    }
    
    private void HandleExpeditionResult(int currentDayLocal)
    {
        int isExpeditionInProgress = PlayerPrefs.GetInt("ExpeditionInProgress");

        if (isExpeditionInProgress == 1)
        {
            int endingDay = PlayerPrefs.GetInt("ExpeditionEndDay");
            if (endingDay <= currentDayLocal)
            {
                //Expedition ended
                int result = PlayerPrefs.GetInt("ExpeditionResult");
                if (result == 1)
                {
                    DataPlayerInventory rewardsData = SaveManager.Instance.TryLoadExpeditionRewardJson();
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
                        Debug.Log("ITEM REWARD DIDNT FIT IN INVENTORY");
                        foreach (var slot in slotUsed)
                        {
                            TrainBaseInventory.Instance.RemoveItemFromItemSlot(slot.Key, slot.Value);
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
    }
}
