using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DataPlayerInventory
{
    public Dictionary<int , int> playerInventory;

    public DataPlayerInventory()
    {
        playerInventory = new Dictionary<int, int>();
    }
    
    public DataPlayerInventory(Dictionary<int, int> items)
    {
        playerInventory = items;
    }

    public Dictionary<int, int> GetInventory()
    {
        return playerInventory;
    }
}
