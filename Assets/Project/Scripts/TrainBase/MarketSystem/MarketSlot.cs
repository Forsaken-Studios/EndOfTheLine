using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MarketSlot : MonoBehaviour, IPointerClickHandler
{
    //TODO: Here we have the item
    private UsableItemSO usableItem;

    private Item itemSO;
    private int amount; 
    public event EventHandler onItemClicked;
    private Sprite emptySprite;
    [SerializeField] private Image itemSlotImage;
    private TextMeshProUGUI amountText;
    [SerializeField] private GameObject priceGameObject;
    private int price;

    private void OnEnable()
    {
        amountText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetUpProperties(UsableItemSO itemInSlot, int amount)
    {
        usableItem = itemInSlot;
        this.itemSlotImage.sprite = itemInSlot.itemIcon;
   
    }

    public void RemoveAmountFromSlot(int amountToRemove)
    {
        this.amount -= amountToRemove;
        this.amountText.text = "x" + amount.ToString();
    }

    public void ClearMarketSlot()
    {
        this.itemSlotImage.sprite = MarketSystem.Instance.GetEmptySprite();
        this.itemSO = null;
        this.amountText.text = "";
        this.amount = 0;
        this.usableItem = null;
        GetComponent<Button>().interactable = false;
    }

    public void SetUpProperties(Item itemSO, int amount)
    {
        amountText = GetComponentInChildren<TextMeshProUGUI>();
        this.itemSO = itemSO;
        this.itemSlotImage.sprite = itemSO.itemIcon;
        this.amountText.text = "";
        this.amount = amount;
    
        priceGameObject.gameObject.SetActive(true);
        price = itemSO.itemPriceAtMarket;
        priceGameObject.GetComponentInChildren<TextMeshProUGUI>().text = price.ToString();
   

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
                Debug.Log("ITEM CLICKED");
                onItemClicked?.Invoke(this, EventArgs.Empty); 
            } 
        }
    }

    public int GetSlotAmount()
    {
        return amount;
    }

    public int GetPrice()
    {
        return price;
    }
}
