using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Expeditions/Expedition Mission", order = 1)]
public class MissionStatSO : ScriptableObject
{
    public int id;
    public string missionName;
    [TextArea(10, 10)]
    public string missionDescription;
    public List<Requirements> requirements;
    public List<MissionRewards> rewards;
    public List<MissionBonusItems> bonusItems;
    public float basicChanceOfSuccess;
    public int daysToComplete;
    [Range(1, 23)]
    public int expeditionMapID;
}

[Serializable]
public class Requirements
{ 
    public RequirementSO requirement;
}

[Serializable]
public class MissionRewards
{
    public ExpeditionRewardSO reward;
}

[Serializable]
public class MissionBonusItems
{
    public MissionBonusItemsSO bonusItems;
}
