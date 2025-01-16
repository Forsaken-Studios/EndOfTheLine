
using System;
using System.Collections.Generic;
using Player;
using UnityEngine;

namespace LootSystem
{
    public class LooteableObjectSelector : MonoBehaviour
    {
        public static LooteableObjectSelector Instance;
    
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("There's more than one LooteableObjectSelector! " + transform + " - " + Instance);
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
    
        //TEST 

        private List<LooteableObject> looteableObjectInRangeList;

        [SerializeField] private GameObject lootSelectorPrefab;
        private GameObject lootSelectorGameObject;
        private LooteableObjectSelectorUI looteableObjectSelectorUI;
        private void Start()
        {
            looteableObjectInRangeList = new List<LooteableObject>();
        }
        
        public void AddOneInTrigger(LooteableObject loot)
        {
            looteableObjectInRangeList.Add(loot);
            if (looteableObjectInRangeList.Count > 1)
            {
                ShowItemSelector();
            }
        }

        public bool GetIfIndexIsThisLooteableObject(LooteableObject looteableObject)
        {
            return looteableObjectInRangeList[looteableObjectSelectorUI.GetCurrentIndex()].Equals(looteableObject); 
        }
    
        public void DecreaseOneInTrigger(LooteableObject loot)
        {
            looteableObjectInRangeList.Remove(loot);
            if (looteableObjectInRangeList.Count < 2)
            {
                HideItemSelector();
            }
        }
    
        private void ShowItemSelector()
        {
            if(lootSelectorGameObject)
                Destroy(lootSelectorGameObject);
            lootSelectorGameObject = Instantiate(lootSelectorPrefab,
                looteableObjectInRangeList[0].gameObject.transform.position, Quaternion.identity);
            looteableObjectSelectorUI = lootSelectorGameObject.GetComponent<LooteableObjectSelectorUI>();
        
        } 
        private void HideItemSelector()
        {
            if (lootSelectorGameObject != null)
            {
                Destroy(lootSelectorGameObject);
                looteableObjectSelectorUI = null;
            }
        }

        public int GetLooteableObjectCount()
        {
            return looteableObjectInRangeList.Count;
        }

        public List<LooteableObject> GetLootList()
        {
            return looteableObjectInRangeList;
        }
        public bool GetIfSelectorIsActive()
        {
            return looteableObjectInRangeList.Count > 1;
        }


        public LooteableObject GetClosestTemporalBox()
        {
            LooteableObject closestTemporalBox = null;
            int minDistance = 3;
            foreach (var lootCrate in looteableObjectInRangeList)
            {
                if (Vector2.Distance(lootCrate.transform.position, PlayerController.Instance.gameObject.transform.position) < minDistance)
                {
                    if (lootCrate.ChestType == LootSpriteContainer.TemporalBox)
                    {
                        return lootCrate;  
                    }
                }
            }
            return closestTemporalBox;
        }
    }
}