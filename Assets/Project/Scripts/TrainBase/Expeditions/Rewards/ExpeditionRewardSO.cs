using System.Collections;
using System.Collections.Generic;
using LootSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Expeditions/Expedition Mission Reward", order = 1)]
public class ExpeditionRewardSO : ScriptableObject
{
    public Item item;
    public int minAmount; 
    public int maxAmount; 
}
