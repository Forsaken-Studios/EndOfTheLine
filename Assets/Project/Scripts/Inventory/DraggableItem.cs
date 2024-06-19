using System.Collections;
using System.Collections.Generic;
using Loot;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils.CustomLogs;

namespace Inventory
{
    public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Image image;
        [HideInInspector] public Transform parentBeforeDrag;
        [HideInInspector] public Transform parentAfterDrag;
        private bool itemFromInventoryToCrate = false;
        private int amountBeforeMoving = 0;
        /// <summary>
        /// Method used when we start draggin an item, saving references from parents and putting outside of his parent
        /// so we can visualize the image around the game. (Not visible behind other images)
        /// </summary>
        /// <param name="eventData"></param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            parentBeforeDrag = transform.parent;
            parentAfterDrag = transform.parent;
            transform.SetParent(transform.root);
            amountBeforeMoving = parentAfterDrag.GetComponentInParent<ItemSlot>().amount;
            transform.SetAsLastSibling();
            image.raycastTarget = false;
        }

        /// <summary>
        /// Image movement when dragging
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Input.mousePosition;
        }

        /// <summary>
        /// Checking if it is the same position as before, if not, we change the parentAfterDrag
        /// Remanining checks done in itemSlot onDrop
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {

            if (parentAfterDrag == parentBeforeDrag)
            {
              /* throw items to the ground, we should instantiate a looteableObject
               ItemSlot itemSlotMoving = parentBeforeDrag.GetComponentInParent<ItemSlot>();
                Debug.Log("TIRAMOS "  + itemSlotMoving.amount + " " + itemSlotMoving.GetItemInSlot().itemName);
                PlayerInventory.Instance.RemovingItem(itemSlotMoving.GetItemInSlot(), itemSlotMoving.amount);
                itemSlotMoving.ClearItemSlot();
                */
                transform.SetParent(parentBeforeDrag);
                this.transform.position = parentBeforeDrag.position;
            }
            else
            {
                //Remember we swap images, so the one that check if it is in the
                //If it comes from loot crate
                ItemSlot itemSlotMoving = parentBeforeDrag.GetComponentInParent<ItemSlot>();
                //And moves to our inventory
                ItemSlot itemSlotFinal = parentAfterDrag.GetComponentInParent<ItemSlot>();
                //Si movemos desde loot a inventario, añadimos objeto a inventario
                if (itemSlotMoving.GetIfIsLootCrate() && !itemSlotFinal.GetIfIsLootCrate())
                {
                    //LootUIManager.Instance.GetCurrentLootableObject().DeleteItemFromList(itemSlotFinal.GetItemInSlot(), itemSlotFinal.amount);
                    //Check if we need to destroy the bag, but actually we wont need to do it, because we will have crates, not bags
                }
                else
                {
                    if (!itemSlotMoving.GetIfIsLootCrate() && itemSlotFinal.GetIfIsLootCrate()
                        && itemFromInventoryToCrate)
                    {
                        //Then we are from our inventory to crate, later with slide
                        ItemSlot itemSlot = parentAfterDrag.GetComponent<ItemSlot>();
                    }
                }
                if (amountBeforeMoving == itemSlotFinal.amount)
                {
                    transform.SetParent(parentBeforeDrag);
                    //this.transform.position = parentAfterDrag.position;  
                }
                else
                {
                    transform.SetParent(parentBeforeDrag);
                   //this.transform.position = parentBeforeDrag.position;  
                }
            }
            transform.SetAsFirstSibling();
            image.raycastTarget = true;
        }

        public void SetItemComingFromInventoryToCrate(bool aux)
        {
            this.itemFromInventoryToCrate = aux; 
        }
              
    
  
    }
}
