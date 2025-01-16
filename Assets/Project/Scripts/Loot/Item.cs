using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

namespace LootSystem
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Item", order = 1)]
    [Serializable]
    public class Item : ScriptableObject
    {
        public Sprite itemIcon;
        public ItemType ItemType;
        public bool canSpawnInEnemyBodies = true;
        public bool canSpawnInCrates = true;
        public int itemPriceAtMarket;
        public float itemWeight;
        public string itemName;
        public float itemSpawnChance;
        [TextAreaAttribute(10, 10)]
        public string itemDescription;
        public int itemID;
    }
}