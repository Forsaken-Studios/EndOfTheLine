using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardIcon : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI amountText;
    
    public void SetUpProperties(ExpeditionRewardSO rewardSO)
    {
        this.itemImage.sprite = rewardSO.item.itemIcon;
        if (rewardSO.minAmount == rewardSO.maxAmount)
        {
            this.amountText.text = rewardSO.minAmount.ToString();
        }
        else
        {
            this.amountText.text = rewardSO.minAmount.ToString() + "-" + rewardSO.maxAmount.ToString();
        }
       
    }
}
