using System;
using System.Collections;
using System.Collections.Generic;
using Inventory;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContextMenu : MonoBehaviour, IDeselectHandler, IPointerClickHandler, IDragHandler
{

    [SerializeField] private Button useButton;
    [SerializeField] private Button inspectButton;
    [SerializeField] private Button discardButton;
    [SerializeField] private GameObject backgroundPanel;

    private ItemSlot itemSlot;
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
    }

    private void InspectItem()
    {
        Debug.Log("INSPECT ITEM");
    }

    private void DiscardItem()
    {
        Debug.Log("DISCARD ITEM " + itemSlot.GetItemInSlot().itemName);
    }

    public void SetItemSlotProperties(ItemSlot itemSlot)
    {
        this.itemSlot = itemSlot;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        //Mouse was clicked outside
      //  Debug.Log("DESTROY CONTEXT MENU");
       // Destroy(this.gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("POINT CLICK NO IF"); 
        Debug.Log(eventData.pointerClick);

        if (eventData.pointerPress.gameObject.CompareTag("BackgroundToDeleteContextMenu"))
        {
            Destroy(this.gameObject);
        }
    }
    

    public void OnDrag(PointerEventData eventData)
    {
        Destroy(this.gameObject);
    }
}
