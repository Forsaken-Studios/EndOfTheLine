using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Expeditions/Expedition", order = 1)]
public class ExpeditionSO : ScriptableObject
{

    public int stationID;
    public string stationName;
    [TextArea(10,10)]
    public string stationDescription;
    public Sprite expeditionImage;
    public string stationDifficulty;


    public List<Mission> missionAvailableInStation;
}

[Serializable]
public class Mission
{
    public MissionStatSO statModifier;
    public float minChance;
}