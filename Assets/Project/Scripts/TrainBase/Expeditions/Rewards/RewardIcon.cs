using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardIcon : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI amountText;

    private ExpeditionRewardSO expeditionReward;
    public void SetUpProperties(ExpeditionRewardSO rewardSO)
    {
        this.expeditionReward = rewardSO;
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

    public void WriteNewRewards(float multiplier)
    {
        if (expeditionReward.minAmount == expeditionReward.maxAmount)
        {
            this.amountText.text = expeditionReward.minAmount.ToString();
        }
        else
        {
            this.amountText.text = (int)(expeditionReward.minAmount * multiplier) + "-" +
                                   (int)(expeditionReward.maxAmount * multiplier);
        } 
    }
}
