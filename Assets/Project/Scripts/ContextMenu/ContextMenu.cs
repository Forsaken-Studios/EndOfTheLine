using System;
using System.Collections;
using System.Collections.Generic;
using Inventory;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ContextMenu
{


    public class ContextMenu : MonoBehaviour, IDeselectHandler, IPointerClickHandler, IDragHandler
    {

        [SerializeField] private Button useButton;
        [SerializeField] private Button inspectButton;
        [SerializeField] private Button discardButton;
        [SerializeField] private GameObject backgroundPanel;

        [SerializeField] private GameObject inspectItemPrefab;
        private ItemSlot itemSlot;
        private Item item;

        private Vector2 initialPosition = new Vector2(960, 540);
        private Vector2 offsetPosition = new Vector2(50, 50);

        private void Awake()
        {
            //EventSystem.current.SetSelectedGameObject(this.gameObject);
        }

        private void Start()
        {
            useButton.onClick.AddListener(() => UseItem());
            inspectButton.onClick.AddListener(() => InspectItem());
            discardButton.onClick.AddListener(() => DiscardItem());
        }

        private void DestroyContextMenu()
        {
            Debug.Log("DESTROY CONTEXT MENU");
        }

        private void UseItem()
        {
            Debug.Log("USE ITEM");
            InventoryManager.Instance.TryDestroyContextMenu();
        }

        private void InspectItem()
        {
            Debug.Log("INSPECT ITEM");
            Debug.Log("ESTAMOS INSPECCIONANDO: " + item.itemName.ToString());
            InventoryManager.Instance.TryDestroyContextMenu();
            //TODO: When inspecting more elements, we will spawn it, just a bit to the right
            Vector2 spawnPosition =
                initialPosition + (offsetPosition * InventoryManager.Instance.GetInspectViewList().Count);
            GameObject inspectItemView = Instantiate(inspectItemPrefab, spawnPosition,
                Quaternion.identity, GameManager.Instance.GetCanvasParent().transform);
            inspectItemView.GetComponent<InspectItemView>().SetUpInspectView(item);
            //TODO: Add this inspect view to list (escape button spam)
            InventoryManager.Instance.AddInspectView(inspectItemView);
        }

        private void DiscardItem()
        {
            itemSlot.ThrowItemToGround();
            InventoryManager.Instance.TryDestroyContextMenu();
        }

        public void SetItemSlotProperties(ItemSlot itemSlot)
        {
            this.itemSlot = itemSlot;
            this.item = this.itemSlot.GetItemInSlot();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            //Mouse was clicked outside
            //  Debug.Log("DESTROY CONTEXT MENU");
            // Destroy(this.gameObject);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.pointerPress.gameObject.CompareTag("BackgroundToDeleteContextMenu"))
            {
                Debug.Log("CLICK OUTSIDE");
                Destroy(this.gameObject);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            Destroy(this.gameObject);
        }
    }
}
