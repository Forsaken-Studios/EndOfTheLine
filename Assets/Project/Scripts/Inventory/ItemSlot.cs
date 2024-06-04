using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

namespace Inventory
{

    public class ItemSlot : MonoBehaviour, IDropHandler
    {
        private Sprite emptySprite;
        [SerializeField] private Image itemSlotImage;
        private TextMeshProUGUI itemSlotAmountText;
        private int ItemID;
        private Item itemInSlot;
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
            this.itemSlotAmountText = this.GetComponentInChildren<TextMeshProUGUI>(includeInactive: true);
            this.itemSlotAmountText.text = "";
             emptySprite = (Sprite) UnityEngine.Resources.Load<Sprite>("Sprites/EmptySprite");
            Debug.Log(emptySprite);
        }

        /// <summary>
        /// Initialize item slot
        /// </summary>
        /// <param name="itemImage"></param>
        /// <param name="itemSlotAmount"></param>
        /// <param name="itemID"></param>
        public void SetItemSlotProperties(Item item, int itemSlotAmount)
        {
            this.itemInSlot = item;
            this.itemSlotImage.sprite = item.itemIcon;
            this.itemID = item.itemID;
            this.itemSlotAmountText.text = "x" + itemSlotAmount.ToString();
            this.amount = itemSlotAmount;
        }

        /// <summary>
        /// Clear Item Slot properties
        /// </summary>
        public void ClearItemSlot()
        {
            Debug.Log(emptySprite);
            this.itemInSlot = null;
            this.itemSlotImage.sprite = emptySprite;
            this.itemID = 0;
            this.itemSlotAmountText.text = "";
            this.amount = 0; 
        }
        
        /// <summary>
        /// Add items to a used slot.
        /// </summary>
        /// <param name="amount"></param>
        public void AddMoreItemsToSameSlot(int amount)
        {
            this.amount += amount;
            this.itemSlotAmountText.text = "x" + this.amount;
        }


        /// <summary>
        /// Method to change itemSlotImage reference.
        /// </summary>
        /// <param name="newImage"></param>
        public void ChangeImage(GameObject newImage)
        {
            newImage.transform.SetParent(this.gameObject.transform);
            this.itemSlotImage = newImage.GetComponent<Image>();
        }


        /// <summary>
        /// Method when we drop an item in a itemSlot
        /// We swaps images, amounts and itemIDs. 
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrop(PointerEventData eventData)
        {

            GameObject dropped = eventData.pointerDrag;
            DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
            if (this.itemID == 0)
            {
                ItemSlot itemSlotBeforeDrop = draggableItem.parentBeforeDrag.GetComponent<ItemSlot>();
                //Cambiamos la imagen de la que estaba aqui, y la cantidad, y la mandamos al otro lado
                itemSlotBeforeDrop.ChangeImage(itemSlotImage.gameObject);
                int auxAmount = itemSlotBeforeDrop.amount;
                int auxID = itemSlotBeforeDrop.itemID;
                itemSlotBeforeDrop.amount = 0;
                itemSlotBeforeDrop.itemID = 0;
                itemSlotBeforeDrop.itemSlotAmountText.text = "";
                //Ahora ponemos la que hemos movido aqui
                itemSlotAmountText.text = "x" + auxAmount.ToString();
                itemSlotImage.gameObject.transform.position = draggableItem.parentBeforeDrag.position;
                amount = auxAmount;
                itemID = auxID;
                this.ChangeImage(draggableItem.GetComponent<Image>().gameObject);
                draggableItem.parentAfterDrag = transform;
            }

        }
    }
}