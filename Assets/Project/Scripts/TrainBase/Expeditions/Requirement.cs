using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Requirement : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI amountText;


    public void SetUpProperties(RequirementSO requirementSO)
    {
        this.itemImage.sprite = requirementSO.item.itemIcon;
        this.amountText.text = "x" + requirementSO.amountNeeded.ToString() + " " + requirementSO.item.itemName;
    }
}
