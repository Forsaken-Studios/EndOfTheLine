using System;
using System.Collections;
using System.Collections.Generic;
using LootSystem;
using TMPro;
using UnityEngine;

public class HoverItem : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemWeightText;

    public void SetUpHoverView(Item item)
    {
        try
        {
            this.itemNameText.text = item.itemName.ToString();
            this.itemWeightText.text = item.itemWeight.ToString() + " KG";
        }
        catch (Exception e)
        {
            Destroy(this.gameObject);
        }

    }
}
