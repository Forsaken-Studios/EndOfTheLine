using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeIcon: MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemAmount;
    [SerializeField] private Image itemRequirementsMeet;

    [SerializeField] private Sprite greenIcon;
    [SerializeField] private Sprite redIcon;

    private Item item;
    private int amount;
    
    public void SetUpProperties(Item item, int amount, bool isReward)
    {
        this.item = item;
        this.amount = amount;
        
        if (amount == 1)
            itemAmount.text = "";
        else
            itemAmount.text = amount.ToString();
        itemIcon.sprite = item.itemIcon;

        if (!isReward)
        {
            if (TrainBaseInventory.Instance.GetIfItemIsInInventory(item, amount))
                itemRequirementsMeet.sprite = greenIcon;
            else
                itemRequirementsMeet.sprite = redIcon;
        }
        else
        {
            Destroy(itemRequirementsMeet.gameObject);
        }
    }
    
    

    public void CheckIfWeHaveItems()
    {
        if (TrainBaseInventory.Instance.GetIfItemIsInInventory(item, amount))
            itemRequirementsMeet.sprite = greenIcon;
        else
            itemRequirementsMeet.sprite = redIcon;
    }
    
}
