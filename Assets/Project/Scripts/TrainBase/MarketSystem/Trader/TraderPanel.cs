using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TraderPanel : MonoBehaviour
{

    [SerializeField] private GameObject itemRequirementPrefab;
    [SerializeField] private GameObject itemTradePrefab;
    [SerializeField] private GameObject itemTradeGridParent;
    
    void Start()
    {

        SetUpPanel();
        
       
    }

    private void SetUpPanel()
    {
        string resourcesPath = "Trades";
        List<ItemTradeSO> itemTrades =
                 UnityEngine.Resources.LoadAll<ItemTradeSO>(resourcesPath).ToList();

        foreach (var trade in itemTrades)
        {
            GameObject tradeGameObject = Instantiate(itemTradePrefab, Vector2.zero, Quaternion.identity,
                itemTradeGridParent.transform);
            tradeGameObject.GetComponentInChildren<TextMeshProUGUI>().text = trade.daysToComplete.ToString() + " days";
            //Set up trade
            SetUpUniqueTradeRequirements(trade, tradeGameObject);
            SetUpTradeRewards(trade, tradeGameObject);
        }
    }

    private void SetUpUniqueTradeRequirements(ItemTradeSO trade, GameObject tradeParent)
    {
        GameObject grid = tradeParent.GetComponentInChildren<GridLayoutGroup>().gameObject;

        foreach (var requirement in trade.requirements)
        {
            GameObject tradeRequirement = Instantiate(itemRequirementPrefab, Vector2.zero, Quaternion.identity, grid.transform);
            tradeRequirement.transform.SetAsFirstSibling();
            tradeRequirement.GetComponent<TradeIcon>().SetUpProperties(requirement.requirement.item, requirement.requirement.amountNeeded, false);
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
