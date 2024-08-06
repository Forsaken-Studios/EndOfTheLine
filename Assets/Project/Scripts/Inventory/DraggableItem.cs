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
        private bool isSplitting = false;
        /// <summary>
        /// Method used when we start draggin an item, saving references from parents and putting outside of his parent
        /// so we can visualize the image around the game. (Not visible behind other images)
        /// </summary>
        /// <param name="eventData"></param>
        public void OnBeginDrag(PointerEventData eventData)
        {

            if(InventoryManager.Instance != null)
                InventoryManager.Instance.TryDestroyContextMenu();
            if (Input.GetKey(KeyCode.LeftControl))
            {
                isSplitting = true; 
            }
            else
            {
                isSplitting = false;
            }
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
            if (Input.GetKey(KeyCode.LeftControl))
            {
                isSplitting = true; 
            }
            else
            {
                isSplitting = false;
            }
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
                transform.SetParent(parentBeforeDrag);
                this.transform.position = parentBeforeDrag.position;
            }
            else
            {
                    ItemSlot itemSlotMoving = parentBeforeDrag.GetComponentInParent<ItemSlot>();
                    //And moves to our inventory
                    ItemSlot itemSlotFinal = parentAfterDrag.GetComponentInParent<ItemSlot>();
                    transform.SetParent(parentBeforeDrag);
                    this.transform.position = parentBeforeDrag.position;
                    SoundManager.Instance.ActivateSoundByName(SoundAction.Inventory_MoveItem);
            }
            transform.SetAsFirstSibling();
            image.raycastTarget = true;
        }

        public void SetItemComingFromInventoryToCrate(bool aux)
        {
            this.itemFromInventoryToCrate = aux; 
        }

        public bool GetIfIsSplitting()
        {
            return isSplitting;
        }
        
    
  
    }
}
