using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DataPlayerInventory
{
    public Dictionary<int , int> itemsSaved;

    public DataPlayerInventory()
    {
        itemsSaved = new Dictionary<int, int>();
    }
    
    public DataPlayerInventory(Dictionary<int, int> items)
    {
        itemsSaved = items;
    }

    public Dictionary<int, int> GetInventory()
    {
        return itemsSaved;
    }
}
