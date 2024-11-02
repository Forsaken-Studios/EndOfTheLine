using System;
using System.Collections;
using System.Collections.Generic;
using LootSystem;
using Player;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Utils.CustomLogs;

namespace Inventory
{

    public class InventoryManager : IInventoryManager
    {
        public static InventoryManager Instance;

        [Header("Inventory Panels")]
        [SerializeField] private TextMeshProUGUI weightText;
        [SerializeField] private GameObject looteableObjectPrefab;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("There's more than one InventoryManager! " + transform + " - " + Instance);
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        
        public override void Start()
        {
            base.Start();
            ChangeText();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                CancelAbilities();
                ReverseInventoryStatus();
                if(LootUIManager.Instance.GetIfCrateIsOpened())
                    LootUIManager.Instance.DesactivateLootUIPanel();
            }

            if (inventoryIsOpen)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (GetInspectViewList().Count != 0)
                    {

                        GameObject mostRecentInspectView = inspectListViewList[inspectListViewList.Count - 1];
                        Destroy(mostRecentInspectView);
                        RemoveInspectView(mostRecentInspectView);
                    }else if (splittingViewActivated)
                    {
                        if(splittingView != null)
                            splittingView.gameObject.SetActive(false);
                        splittingViewActivated = false;
                    }
                    else
                    {
                        ReverseInventoryStatus();
                        if(LootUIManager.Instance.GetIfCrateIsOpened())
                            LootUIManager.Instance.DesactivateLootUIPanel(); 
                    }
                }
            }
        }

        private void CancelAbilities()
        {
            AbilityManager.Instance.Holder1.TryToCancelAbility();
            AbilityManager.Instance.Holder2.TryToCancelAbility();
        }
        
        /// <summary>
        /// If inventory is opened, we close it, if it is the other way, we open it
        /// </summary>
        public void ReverseInventoryStatus()
        {
            if (!inventoryHUD.activeSelf)
            {
                CameraSingleton.CameraSingletonInstance.ZoomCameraOnInventory();
            }
            else
            {
                CameraSingleton.CameraSingletonInstance.UnZoomToNormalPosition();
            }

            base.ReverseInventoryStatus();
        }
        public void ActivateInventory()
        {
            CameraSingleton.CameraSingletonInstance.ZoomCameraOnInventory();
            base.ActivateInventory();
        }
        public void DesactivateInventory()
        {
            CameraSingleton.CameraSingletonInstance.UnZoomToNormalPosition();
            base.DesactivateInventory();
        }
        public void ChangeText()
        {
            weightText.text = PlayerController.Instance.CurrentWeight.ToString("F2") + " / " +
                              PlayerController.Instance.GetMaxWeight() + " KG";
            
            if (PlayerController.Instance.CurrentWeight >= PlayerController.Instance.GetMaxWeight())
            {
                weightText.color = Color.red;
            }else if (PlayerController.Instance.CurrentWeight >= PlayerController.Instance.GetOverweight())
            {
                weightText.color = Color.yellow;
            }
            else
            {
                weightText.color = Color.white;
            }
        }
        /// <summary>
        /// Methods that finds and set the first available spot for item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <param name="remainingItemsWithoutSpace"></param>
        /// <returns></returns>
        public bool TryAddInventoryToItemSlot(Item item, int amount, out int remainingItemsWithoutSpace)
        {
            return base.TryAddInventoryToItemSlot(item, amount, out remainingItemsWithoutSpace);
        }
        
        public GameObject GetLooteableObjectPrefab()
        {
            return looteableObjectPrefab; 
        }


    }
}

