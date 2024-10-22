using System.Collections;
using System.Collections.Generic;
using Inventory;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableAbilityIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Image image;
        [HideInInspector] public Transform parentBeforeDrag;
        [HideInInspector] public Transform parentAfterDrag;
        [SerializeField] private AbilityIconEquipment ability;
        
        
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
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            if (parentAfterDrag != parentBeforeDrag)
            {
                
                //Equip ability in slot
                ItemSlot itemSlotMoving = parentBeforeDrag.GetComponentInParent<ItemSlot>();
                //And moves to our inventory
                ItemSlot itemSlotFinal = parentAfterDrag.GetComponentInParent<ItemSlot>();
                transform.SetParent(parentBeforeDrag);
                this.transform.position = parentBeforeDrag.position;
                SoundManager.Instance.ActivateSoundByName(SoundAction.Inventory_MoveItem, null, true);
            }
            
            transform.SetParent(parentBeforeDrag);
            this.transform.position = parentBeforeDrag.position;
            transform.SetSiblingIndex(2);
            image.raycastTarget = true;
        }

        public Ability GetAbility()
        {
            return ability.GetAbility();
        }
}
