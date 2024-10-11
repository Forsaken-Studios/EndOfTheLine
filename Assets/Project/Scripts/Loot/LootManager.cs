using System;
using System.Collections;
using System.Collections.Generic;
using Loot;
using UnityEngine;

public class LootManager : MonoBehaviour
{

    public static LootManager Instance;
    [SerializeField] private int totalCratesAmount = 5;
    [SerializeField] private float totalEmptyCratesPercentage = 0.3f;
    [SerializeField] private int minNumberOfItemsToSpawn = 1; 
    [SerializeField] private int maxNumberOfItemsToSpawn = 4; //6 = MAX SLOT
    
    
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
        PrepareLoot();
    }

    public int GetRandomAmount()
    {
        return UnityEngine.Random.Range(minNumberOfItemsToSpawn, maxNumberOfItemsToSpawn);
    }
    
    private void PrepareLoot()
    {
        int cratesToFill = (int)((totalCratesAmount) - (totalCratesAmount * totalEmptyCratesPercentage));
        //First we check if we need to spawn an specific item
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
    }


    public void AddLooteableObjectToList(LooteableObject looteableObject)
    {
        this.cratesList.Add(looteableObject);
    }
}
