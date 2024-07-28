using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MarketSlot : MonoBehaviour, IPointerClickHandler
{
    //TODO: Here we have the item
    private UsableItemSO item;

    public event EventHandler onItemClicked;


    public void SetUpProperties(UsableItemSO itemInSlot)
    {
        item = itemInSlot;
    }
    
    public string GetItemName()
    {
        return item.itemName;
    }
    
    public UsableItemSO GetItem()
    {
        return item;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("ITEM CLICKED");
        onItemClicked?.Invoke(this, EventArgs.Empty);
    }
}
