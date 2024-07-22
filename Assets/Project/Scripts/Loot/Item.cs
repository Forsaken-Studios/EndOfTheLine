using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Item", order = 1)]
public class Item : ScriptableObject
{
    public Sprite itemIcon;
    public ItemType ItemType;
    public int itemValue;
    public float itemWeight;
    public string itemName;
    public string itemDescription;
    public int itemID;
}


