using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;
using Random = UnityEngine.Random;

namespace Loot
{


    public class LooteableObject : MonoBehaviour
    {

        [SerializeField] private GameObject hotkeyImage;
        private Dictionary<Item, int> itemsInLootableObject;
        private LooteableObjectUI looteableObjectUI;
        [SerializeField] private bool onlyOneItemInBag;
        private bool _isLooteable = false;
        private bool isLooting = false;

        
        
        public bool IsLooteable
        {
            get { return _isLooteable; }
            set { _isLooteable = value; }
        }

        private void Start()
        {
            itemsInLootableObject = new Dictionary<Item, int>();
            looteableObjectUI = GetComponent<LooteableObjectUI>();
            PrepareLoot();
        }

        private void PrepareLoot()
        {
            //TODO: AQUI SE ELIGE QUE TIPO DE LOOT PODRIAMOS PONER (AHORA ES TODOS LOS ITEMS)
            Object[] allItems = UnityEngine.Resources.LoadAll("Items");
            List<Object> allItemsList = allItems.ToList();
            int itemsToLoot = 1;
            if (!onlyOneItemInBag)
                itemsToLoot = Random.Range(2, looteableObjectUI.GetMaxSlotsInCrate());
                //itemsToLoot = Random.Range(2, 3); NO PANEL
            for (int i = 0; i < itemsToLoot; i++)
            {
                int randomItemIndex = Random.Range(0, allItemsList.Count);
                int randomQuantity = Random.Range(1, 4);
                Item itemSO = allItemsList[randomItemIndex] as Item;
                //Debug.Log("ITEM IN LOOTABLE OBJECT: " + itemSO.name + " -> x" + randomQuantity);
                itemsInLootableObject.Add(itemSO, randomQuantity);
                //WE MODIFIE THE UI
                looteableObjectUI.AddItemToCrate(itemSO, randomQuantity);
                allItemsList.RemoveAt(randomItemIndex);
            }
        }

        private void Update()
        {
            if (_isLooteable)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    //Loot
                    if (looteableObjectUI.GetIfCrateIsOpened())
                    {
                        looteableObjectUI.DesactivateLooteablePanel(); 
                    }
                    else
                    {
                        looteableObjectUI.ActivateLooteablePanel();
                    }
                   
                   /* foreach (var item in itemsInLootableObject)
                    {
                        //PlayerInventory.Instance.TryAddItem(item.Key as Item, item.Value);
                    }
                    
                    //InventoryManager.Instance.ChangeText(PlayerInventory.Instance.GetInventoryItems());
                    //TODO: Ahora mismo, esto no funciona, pero si looteamos por raton, esto no haría falta, hay que hacerlo de la otra forma.
                        //Destroy(this.gameObject);
                        //_isLooteable = false;
                    
                    */
                }
            }
        }

        public void ActivateKeyHotkeyImage()
        {
            hotkeyImage.SetActive(true);
            _isLooteable = true;
        }

        public void DesactivateKeyHotkeyImage()
        {
            hotkeyImage.SetActive(false);
            _isLooteable = false;
        }
    }
}