using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Passive Upgrade", order = 1)]
public class UpgradesSO : ScriptableObject
{
    // Start is called before the first frame update
    public new string name;
    [TextAreaAttribute(10, 10)]
    public new string description;
    public Sprite upgradeIcon;
    public int upgradeID;
    public float material1Cost;
    public float material2Cost;
    public float material3Cost;
}
