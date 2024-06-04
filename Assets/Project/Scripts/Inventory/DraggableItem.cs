using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inventory
{
    public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Image image;
        [HideInInspector] public Transform parentBeforeDrag;
        [HideInInspector] public Transform parentAfterDrag;

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
                transform.SetParent(parentAfterDrag);
                this.transform.position = parentAfterDrag.position;
            }

            transform.SetAsFirstSibling();
            image.raycastTarget = true;
        }
    }
}
