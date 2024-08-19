using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class TraderPanel : MonoBehaviour
{

    [SerializeField] private GameObject itemRequirementPrefab;
    [SerializeField] private GameObject itemTradePrefab;
    [SerializeField] private GameObject itemTradeGridParent;

    private List<Button> buttonList;
    private StartTradeButton tradeButtonInProgress;
    private List<TradeIcon> iconList;
    private bool tradeInProgress = false;
    void Start()
    {
        buttonList = new List<Button>();
        iconList = new List<TradeIcon>();
        tradeInProgress = PlayerPrefs.GetInt("TradeInProgress") == 1;
        SetUpPanel();
        StartTradeButton.onTradeStarted += OnTradeStarted;
        StartTradeButton.onTradeEnded += OnTradeEnded;
    }

    private void OnEnable()
    {
        if (iconList != null)
        {
            foreach (var icon in iconList)
            {
                icon.CheckIfWeHaveItems();
            }
        }
    }


    private void OnDestroy()
    {
        StartTradeButton.onTradeStarted -= OnTradeStarted;
        StartTradeButton.onTradeEnded -= OnTradeEnded;
    }
    
    private void OnTradeStarted(object sender, ItemTradeSO e)
    {
        int currentDay = PlayerPrefs.GetInt("CurrentDay");
        int endDay = e.daysToComplete + currentDay;
        PlayerPrefs.SetInt("TradeEndDay", endDay);
        PlayerPrefs.SetInt("TradeRewardID", e.itemReceived.item.itemReceived.itemID);
        PlayerPrefs.SetInt("TradeInProgress", 1);

        MakeButtonsNotInteractable();
    }
    private void OnTradeEnded(object sender, EventArgs e)
    {
        MakeButtonsInteractable();
    }
    private void MakeButtonsNotInteractable()
    {
        foreach (var button in buttonList)
        {
            button.interactable = false;
        }
    }  
    private void MakeButtonsInteractable()
    {
        foreach (var button in buttonList)
        {
            Debug.Log(button);
            button.interactable = true;
        }
    }
    
    private void SetUpPanel()
    {
        string resourcesPath = "Trades";
        List<ItemTradeSO> itemTrades =
                 UnityEngine.Resources.LoadAll<ItemTradeSO>(resourcesPath).ToList();

        int id = 0;
        foreach (var trade in itemTrades)
        {
            GameObject tradeGameObject = Instantiate(itemTradePrefab, Vector2.zero, Quaternion.identity,
                itemTradeGridParent.transform);
            tradeGameObject.GetComponentInChildren<TextMeshProUGUI>().text = trade.daysToComplete.ToString() + " days";
         
            //Set up trade
            SetUpUniqueTradeRequirements(trade, tradeGameObject);
            SetUpTradeRewards(trade, tradeGameObject);

            //Set Up Buttons 
            Button button = tradeGameObject.GetComponentInChildren<Button>();
            buttonList.Add(button);
            button.GetComponent<StartTradeButton>().SetUpProperties(trade, id);
            HandleTradeButtonInProgress(id, button);
            id++;
            button.gameObject.transform.SetAsLastSibling();
        }
    }

    private void HandleTradeButtonInProgress(int id, Button button)
    {
        bool rewardPickedUp = PlayerPrefs.GetInt("TradeID") != -1;
        if (tradeInProgress && rewardPickedUp)
        {
            int tradeID = PlayerPrefs.GetInt("TradeID");
            Debug.Log("ID: " + id);
            Debug.Log("TRADE ID: " + tradeID);
            if (id == tradeID)
            {
                int currentDay = PlayerPrefs.GetInt("CurrentDay");
                int dayTradeEnd = PlayerPrefs.GetInt("TradeEndDay");

                if (currentDay == dayTradeEnd)
                {
                    tradeButtonInProgress = button.GetComponent<StartTradeButton>();
                    button.GetComponentInChildren<TextMeshProUGUI>().text = "RECEIVE";
                    tradeButtonInProgress.SetReceivingReward(true);
                    button.interactable = true;
                }
                else
                {
                    button.GetComponentInChildren<TextMeshProUGUI>().text = "IN PROGRESS";
                    button.interactable = false; 
                }
            }
            else
            {
                button.interactable = false;
            }
        }
    }

    private void SetUpUniqueTradeRequirements(ItemTradeSO trade, GameObject tradeParent)
    {
        GameObject grid = tradeParent.GetComponentInChildren<GridLayoutGroup>().gameObject;

        foreach (var requirement in trade.requirements)
        {
            GameObject tradeRequirement = Instantiate(itemRequirementPrefab, Vector2.zero, Quaternion.identity, grid.transform);
            tradeRequirement.transform.SetAsFirstSibling();
            TradeIcon tradeIcon = tradeRequirement.GetComponent<TradeIcon>();
            iconList.Add(tradeIcon);
            tradeIcon.SetUpProperties(requirement.requirement.item, requirement.requirement.amountNeeded, false);
        }
    }

    private void SetUpTradeRewards(ItemTradeSO trade, GameObject tradeParent)
    {
        GameObject grid = tradeParent.GetComponentInChildren<GridLayoutGroup>().gameObject;
        GameObject tradeRequirement = Instantiate(itemRequirementPrefab, Vector2.zero, Quaternion.identity, grid.transform);
        tradeRequirement.transform.SetAsLastSibling();
        tradeRequirement.GetComponent<TradeIcon>().SetUpProperties(trade.itemReceived.item.itemReceived, 1, true);
    }
    
    
}
