using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Expeditions/Expedition", order = 1)]
public class ExpeditionSO : ScriptableObject
{

    public string stationName;
    [TextArea(10,10)]
    public string stationDescription;
    public string stationDifficulty; 
}
