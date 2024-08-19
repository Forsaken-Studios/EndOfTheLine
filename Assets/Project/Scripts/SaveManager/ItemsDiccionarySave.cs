using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemsDiccionarySave
{
    public Dictionary<int , int> itemsSaved;

    public ItemsDiccionarySave()
    {
        itemsSaved = new Dictionary<int, int>();
    }
    
    public ItemsDiccionarySave(Dictionary<int, int> items)
    {
        itemsSaved = items;
    }

    public Dictionary<int, int> GetInventory()
    {
        return itemsSaved;
    }
}
