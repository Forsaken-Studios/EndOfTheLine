using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : ScriptableObject
{

    public new string name;
    [TextAreaAttribute(10, 10)]
    public new string description;
    public Sprite abilityIcon;
    public int abilityID;
    public float cooldownTime;
    public float activeTime;


    public int goldNeededToUnlock;
    public int materialNeededToUnlock;
    public virtual void Activate(GameObject parent) {}
    public virtual void BeginCooldown(GameObject parent){}
}
