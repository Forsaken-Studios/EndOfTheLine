using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Lore", order = 1)]
public class LoreSO : ScriptableObject
{
    public int loreID;
    public string loreTitle;
    [TextArea(10, 100)]
    public string loreDescription;
    public string loreSign;
}
