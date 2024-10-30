using System;
using System.Collections;
using System.Collections.Generic;
using Loot;
using UnityEngine;

namespace LootSystem
{
    public class LootManager : MonoBehaviour
    {

        public static LootManager Instance;
        [SerializeField] private int totalCratesAmount = 5;
        [SerializeField] private float totalEmptyCratesPercentage = 0.3f;
        [SerializeField] private int minNumberOfItemsToSpawn = 1; 
        [SerializeField] private int maxNumberOfItemsToSpawn = 4; //6 = MAX SLOT

        private bool lootInitialized = false;
        public List<LooteableObject> cratesList { get; private set; }


    
    
        private void Awake()
        {

            if (Instance != null)
            {
                Destroy(this);
            }
            Instance = this;
            cratesList = new List<LooteableObject>();
        }


        private void Start()
        {
            
        }

        private void Update()
        {
            if (GameManager.Instance.sceneIsLoading || lootInitialized)
            {
                return;
            }
            
            
            Debug.Log("KW 1: " + GameManager.Instance.sceneIsLoading);
            Debug.Log("KW 2: " + lootInitialized);
            PrepareLoot();
                
        }

        public int GetRandomAmount()
        {
            return UnityEngine.Random.Range(minNumberOfItemsToSpawn, maxNumberOfItemsToSpawn);
        }
    
        private void PrepareLoot()
        {
            totalCratesAmount = cratesList.Count;
            int cratesToFill = (int)((totalCratesAmount) - (totalCratesAmount * totalEmptyCratesPercentage));
            //First we check if we need to spawn an specific item
            //Debug.Log("COUNT KW: " + totalCratesAmount);
            //Debug.Log("COUNT KW2: " + cratesToFill);
            foreach (LooteableObject crate in cratesList)
            {
                if (crate.CheckIfNeedToSpawnXObject)
                {
                    crate.StartSpawingObjects();
                    cratesToFill--;
                }
            }
            foreach (LooteableObject crate in cratesList)
            {
                if (cratesToFill != 0)
                {
                    if (!crate.AlreadyLoadedWithLoot)
                    {
                        crate.StartSpawingObjects();
                        cratesToFill--;
                    }
                }
            }
            lootInitialized = true;
        }


        public void AddLooteableObjectToList(LooteableObject looteableObject)
        {
            this.cratesList.Add(looteableObject);
        }
    }
}