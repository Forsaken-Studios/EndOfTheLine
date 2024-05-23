using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private Sprite emptySprite;
    private Image itemSlotImage;
    private TextMeshProUGUI itemSlotAmountText;
    private int ItemID;
    public int itemID
    {
        get { return ItemID; }
        set { this.ItemID = value; }
    }
    private int Amount;
    public int amount
    {
        get { return Amount; }
        set { this.Amount = value; } 
    }

    private void Awake()
    {
        this.itemSlotImage = this.GetComponentInChildren<Image>(includeInactive:true);
        this.itemSlotAmountText = this.GetComponentInChildren<TextMeshProUGUI>(includeInactive:true);
        this.itemSlotAmountText.text = "";
    }
    

    public void SetItemSlotProperties(Sprite itemImage, int itemSlotAmount, int itemID)
    {
        Debug.Log(this.itemSlotImage);
        this.itemSlotImage.sprite = itemImage;
        this.itemSlotAmountText.text = "x" + itemSlotAmount.ToString();
        this.itemID = itemID;
        this.amount = itemSlotAmount;
    }

    public void AddMoreItemsToSameSlot(int amount)
    {
        this.amount += amount;
        this.itemSlotAmountText.text = "x" + this.amount;
    }

    public void ClearItemSlot()
    {
        this.itemSlotImage.sprite = emptySprite;
        this.itemSlotAmountText.text = "";
        this.itemID = -1;
        this.amount = 0;
    }
}
