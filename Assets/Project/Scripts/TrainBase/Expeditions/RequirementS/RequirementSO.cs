using System.Collections;
using System.Collections.Generic;
using LootSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Requirement", order = 1)]
public class RequirementSO : ScriptableObject
{
    public Item item;
    public int amountNeeded;
    
    
}
