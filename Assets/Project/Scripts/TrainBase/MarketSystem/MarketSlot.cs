using System;
using System.Collections;
using System.Collections.Generic;
using LootSystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MarketSlot : MonoBehaviour, IPointerClickHandler
{
    //TODO: Here we have the item
    private UsableItemSO usableItem;

    private Item itemSO;
    public event EventHandler onItemClicked;
    [SerializeField] private Image itemSlotImage;
    [SerializeField] private GameObject priceGameObject;
    [SerializeField] private GameObject blackPanel;
    private int price;
    private bool alreadyBought = false;
    
    public void RemoveAmountFromSlot(int amountToRemove)
    {
        /*this.amount -= amountToRemove;
        this.amountText.text = "x" + amount.ToString();*/
    }

    public void ClearMarketSlot()
    {
        blackPanel.SetActive(true);
        GetComponent<Button>().interactable = false;
    }

    public void SetUpProperties(Item itemSO, bool alreadyBought)
    {
        if (alreadyBought)
        {
            blackPanel.SetActive(true);
        }
        this.alreadyBought = alreadyBought;
        this.itemSO = itemSO;
        this.itemSlotImage.sprite = itemSO.itemIcon;
        priceGameObject.gameObject.SetActive(true);
        price = itemSO.itemPriceAtMarket;
        priceGameObject.GetComponentInChildren<TextMeshProUGUI>().text = itemSO.itemPriceAtMarket.ToString();

    }
    
    public string GetItemName()
    {
        return itemSO == null ? usableItem.itemName : itemSO.itemName;
    }
    
    public UsableItemSO GetUsableItemSO()
    {
        return usableItem;
    }   
    public Item GetItemSO()
    {
        return itemSO;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (itemSO != null || usableItem != null)
            {
                onItemClicked?.Invoke(this, EventArgs.Empty); 
            } 
        }
    }

    public int GetPrice()
    {
        return price;
    }

    public bool GetIfIsAlreadyBought()
    {
        return alreadyBought;
    }
}
