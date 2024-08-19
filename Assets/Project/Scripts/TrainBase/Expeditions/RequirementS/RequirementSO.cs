using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Expeditions/Mission Requirement", order = 1)]
public class RequirementSO : ScriptableObject
{
    public Item item;
    public int amountNeeded;
    
    
}
