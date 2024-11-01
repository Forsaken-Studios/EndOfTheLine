using System.Collections;
using System.Collections.Generic;
using LootSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Expeditions/Expedition Mission Bonus Items", order = 1)]
public class MissionBonusItemsSO : ScriptableObject
{
    public Item item;
    public float itemsRewardsMultiplier;
    public int increaseChances;
}
