using System;
using System.Collections;
using System.Collections.Generic;
using Inventory;
using UnityEngine;

public class TrainInventoryManager : IInventoryManager
{
    public static TrainInventoryManager Instance;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one TrainInventoryManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ReverseInventoryStatus();
        }
    }
}
