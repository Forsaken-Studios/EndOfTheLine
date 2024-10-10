using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Item", order = 1)]
[Serializable]
public class Item : ScriptableObject
{
    public Sprite itemIcon;
    public ItemType ItemType;
    public int itemValue;
    public int itemPriceAtMarket;
    public float itemWeight;
    public string itemName;
    public float itemSpawnChance;
    public string itemDescription;
    public int itemID;
}



