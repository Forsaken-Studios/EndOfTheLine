using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WagonPrice", order = 1)]
public class WagonPrice : ScriptableObject
{
    public int foodNeeded;
    public int materialNeeded;
    public int goldNeeded;
}
