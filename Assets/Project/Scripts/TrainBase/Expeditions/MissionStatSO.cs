using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Expeditions/Expedition Mission", order = 1)]
public class MissionStatSO : ScriptableObject
{
    public int id;
    public string missionName;
    public List<Requirements> requirements;
    public float basicChanceOfSuccess;
}

[Serializable]
public class Requirements
{ 
    public RequirementSO requirement;
}